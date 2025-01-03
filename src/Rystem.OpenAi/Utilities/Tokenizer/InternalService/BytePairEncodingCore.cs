﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Rystem.OpenAi.Utilities.Tokenizer
{
    internal sealed class BytePairEncodingCore
    {
        public BytePairEncodingCore(
            Dictionary<byte[], int>? bytePairEncoder = null,
            Dictionary<string, int>? specialTokenEncoder = null,
            Regex? tokenPatternRegex = null
        )
        {
            var comparer = new ByteArrayEqualityComparer();
            Encoder = bytePairEncoder == null
                ? new Dictionary<byte[], int>(comparer)
                : new Dictionary<byte[], int>(bytePairEncoder, comparer);
            Decoder = bytePairEncoder?.ToDictionary(pair => pair.Value, pair => pair.Key.ToArray())
                      ?? [];

            SpecialTokensEncoder = specialTokenEncoder ?? [];
            SpecialTokensDecoder =
                specialTokenEncoder?.ToDictionary(pair => pair.Value, pair => Encoding.UTF8.GetBytes(pair.Key))
                ?? [];
            RegexTls = tokenPatternRegex ?? new Regex(string.Empty, RegexOptions.None, TimeSpan.FromSeconds(1));

            var parts = SpecialTokensEncoder.Keys.Select(Regex.Escape).ToArray();
            var joinedParts = string.Join("|", parts);
            try
            {
                SpecialTokenPatternRegex = new Regex(joinedParts, RegexOptions.None, TimeSpan.FromSeconds(1));
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException("Invalid regular expression pattern.", e);
            }
        }

        public Dictionary<byte[], int> Encoder { get; }
        public Dictionary<string, int> SpecialTokensEncoder { get; }
        public Dictionary<int, byte[]> Decoder { get; }
        public Dictionary<int, byte[]> SpecialTokensDecoder { get; }
        public Regex RegexTls { get; }
        public Regex SpecialTokenPatternRegex { get; }

        public BytePairEncodingResponse EncodeNative(string text, ISet<string> allowedSpecial)
        {
            var encodedTokens = new List<int>();
            var allTokens = new List<string>();
            var startIndex = 0;
            var lastTokenLength = 0;

            while (true)
            {
                var nextSpecialStartIndex =
                    FindNextSpecialStartIndex(text, allowedSpecial, startIndex, SpecialTokenPatternRegex);

                var endIndex = nextSpecialStartIndex ?? text.Length;
                var textSegment = text[startIndex..endIndex];

                foreach (var matchedValue in RegexTls.Matches(textSegment).Select(x => x.Value))
                {
                    allTokens.Add(matchedValue);
                    var encodedPiece = Encoding.UTF8.GetBytes(matchedValue);
                    if (Encoder.TryGetValue(encodedPiece, out var token))
                    {
                        lastTokenLength = 1;
                        encodedTokens.Add(token);
                        continue;
                    }

                    var tokens = BytePairEncode(encodedPiece, Encoder).ToList();
                    lastTokenLength = tokens.Count;
                    encodedTokens.AddRange(tokens);
                }

                if (nextSpecialStartIndex.HasValue)
                {
                    var specialToken = text[nextSpecialStartIndex.Value..];
                    var specialTokenValue = SpecialTokensEncoder[specialToken];
                    encodedTokens.Add(specialTokenValue);
                    startIndex = nextSpecialStartIndex.Value + specialToken.Length;
                    lastTokenLength = 0;
                }
                else
                {
                    break;
                }
            }

            return new BytePairEncodingResponse
            {
                EncodedTokens = encodedTokens,
                LastTokenLength = lastTokenLength,
                Tokens = allTokens
            };
        }

        private static int? FindNextSpecialStartIndex(string text, ISet<string> allowedSpecial, int startIndex,
            Regex specialRegex)
        {
            var searchIndex = startIndex;

            while (true)
            {
                var nextSpecialMatch = specialRegex.Match(text, searchIndex);

                if (!nextSpecialMatch.Success)
                {
                    return null;
                }

                var specialToken = nextSpecialMatch.Value;

                if (allowedSpecial.Contains(specialToken))
                {
                    return nextSpecialMatch.Index + searchIndex;
                }

                searchIndex = nextSpecialMatch.Index + searchIndex + 1;
            }
        }

        public List<byte> DecodeNative(int[] tokens)
        {
            var decodedBytes = new List<byte>(tokens.Length * 2);
            foreach (var token in tokens)
            {
                if (!TryDecodeToken(token, out var tokenBytes))
                {
                    continue;
                }

                if (tokenBytes != null)
                {
                    decodedBytes.AddRange(tokenBytes);
                }
            }

            return decodedBytes;
        }

        private bool TryDecodeToken(int token, out byte[]? tokenBytes)
        {
            return Decoder.TryGetValue(token, out tokenBytes) ||
                   SpecialTokensDecoder.TryGetValue(token, out tokenBytes);
        }
        private static T[] BytePairMerge<T>(
            byte[] piece, IReadOnlyDictionary<byte[], int> ranks, Func<Range, T> f)
        {
            var partitions = Enumerable.Range(0, piece.Length + 1)
                .Select(i => (Start: i, Rank: int.MaxValue))
                .ToList();

            for (var i = 0; i < partitions.Count - 2; i++)
            {
                var rank = GetRank(piece, ranks, partitions, i, 0);
                if (rank.HasValue)
                {
                    partitions[i] = (partitions[i].Start, rank.Value);
                }
            }

            CheckAllPartitions(piece, ranks, partitions);

            var output = new List<T>(partitions.Count - 1);
            for (var i = 0; i < partitions.Count - 1; i++)
            {
                output.Add(f(new Range(partitions[i].Start, partitions[i + 1].Start)));
            }

            return [.. output];
        }
        private static void CheckAllPartitions(IReadOnlyCollection<byte> piece,
            IReadOnlyDictionary<byte[], int> ranks,
            List<(int Start, int Rank)> partitions)
        {
            while (partitions.Count > 1)
            {
                var minRank = int.MaxValue;
                var minRankIdx = 0;

                for (var i = 0; i < partitions.Count - 1; i++)
                {
                    if (partitions[i].Rank < minRank)
                    {
                        minRank = partitions[i].Rank;
                        minRankIdx = i;
                    }
                }

                if (minRank != int.MaxValue)
                {
                    partitions[minRankIdx] = (partitions[minRankIdx].Start,
                        GetRank(piece, ranks, partitions, minRankIdx, 1) ?? int.MaxValue);

                    if (minRankIdx > 0)
                    {
                        partitions[minRankIdx - 1] = (partitions[minRankIdx - 1].Start,
                            GetRank(piece, ranks, partitions, minRankIdx - 1, 1) ?? int.MaxValue);
                    }

                    partitions.RemoveAt(minRankIdx + 1);
                }
                else
                    break;
            }
        }
        private static int? GetRank(IReadOnlyCollection<byte> piece, IReadOnlyDictionary<byte[], int> ranks,
            List<(int Start, int Rank)> partitionsList, int startIndex, int skip)
        {
            if (startIndex + skip + 2 >= partitionsList.Count)
            {
                return null;
            }

            var key = piece.Skip(partitionsList[startIndex].Start)
                .Take(partitionsList[startIndex + skip + 2].Start - partitionsList[startIndex].Start)
                .ToArray();

            return ranks.TryGetValue(key, out var rank) ? rank : (int?)null;
        }
        private static int[] BytePairEncode(byte[] inputBytes, Dictionary<byte[], int> bytePairRanks)
        {
            if (inputBytes.Length == 1)
            {
                return [bytePairRanks[inputBytes]];
            }

            return BytePairMerge(inputBytes, bytePairRanks, pair =>
            {
                var key = inputBytes.Skip(pair.Start.Value).Take(pair.End.Value - pair.Start.Value).ToArray();
                return bytePairRanks[key];
            });
        }
    }

    internal sealed class ByteArrayEqualityComparer : IEqualityComparer<byte[]>
    {
        public bool Equals(byte[]? x, byte[]? y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            return ReferenceEquals(x, y) || StructuralComparisons.StructuralEqualityComparer.Equals(x, y);
        }

        public int GetHashCode(byte[] obj)
        {
            return StructuralComparisons.StructuralEqualityComparer.GetHashCode(obj);
        }
    }
}
