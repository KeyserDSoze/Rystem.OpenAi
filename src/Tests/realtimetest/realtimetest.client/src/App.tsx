import React, { useEffect, useRef, useState } from 'react';

const RealtimeChat: React.FC = () => {
    // Reference for the audio element (to play remote audio)
    const audioRef = useRef<HTMLAudioElement>(null);
    // Reference for the data channel so we can send chat events
    const dataChannelRef = useRef<RTCDataChannel | null>(null);
    // Reference for the RTCPeerConnection (for cleanup, etc.)
    const pcRef = useRef<RTCPeerConnection | null>(null);
    // Track connection state for UI feedback
    const [isConnected, setIsConnected] = useState(false);

    useEffect(() => {
        const initWebRTC = async () => {
            try {
                // 1. Fetch the ephemeral token from your C# API.
                const tokenResponse = await fetch('http://localhost:7166/api/getToken');
                const tokenData = await tokenResponse.json();
                const EPHEMERAL_KEY = tokenData.client_secret.value;

                // 2. Create a new RTCPeerConnection.
                const pc = new RTCPeerConnection();
                pcRef.current = pc;

                // 3. When a remote audio track is received, assign it to the audio element.
                pc.ontrack = (event) => {
                    if (audioRef.current && event.streams && event.streams[0]) {
                        audioRef.current.srcObject = event.streams[0];
                    }
                };

                // 4. Get microphone audio from the user and add the audio track to the connection.
                const localStream = await navigator.mediaDevices.getUserMedia({ audio: true });
                localStream.getTracks().forEach((track) => {
                    pc.addTrack(track, localStream);
                });

                // 5. Create a data channel for sending and receiving events.
                const dataChannel = pc.createDataChannel('oai-events');
                dataChannelRef.current = dataChannel;
                dataChannel.onopen = () => {
                    console.log('Data channel is open');
                };
                dataChannel.onmessage = (event) => {
                    try {
                        const realtimeEvent = JSON.parse(event.data);
                        console.log('Received realtime event:', realtimeEvent);
                    } catch (error) {
                        console.error('Error parsing message from data channel:', error);
                    }
                };

                // 6. Create an SDP offer and set it as the local description.
                const offer = await pc.createOffer();
                await pc.setLocalDescription(offer);

                // 7. Send the SDP offer to the Realtime API using the ephemeral token.
                const baseUrl = 'https://api.openai.com/v1/realtime';
                const model = 'gpt-4o-realtime-preview-2024-12-17';
                const sdpResponse = await fetch(`${baseUrl}?model=${model}`, {
                    method: 'POST',
                    headers: {
                        Authorization: `Bearer ${EPHEMERAL_KEY}`,
                        'Content-Type': 'application/sdp',
                    },
                    body: offer.sdp,
                });

                // 8. Parse the answer SDP from the API and set it as the remote description.
                const answerSdp = await sdpResponse.text();
                const answer: RTCSessionDescriptionInit = { type: 'answer', sdp: answerSdp };
                await pc.setRemoteDescription(answer);

                setIsConnected(true);
            } catch (error) {
                console.error('Error during WebRTC setup:', error);
            }
        };

        initWebRTC();

        // Cleanup: close the connection when the component unmounts.
        return () => {
            if (pcRef.current) {
                pcRef.current.close();
            }
        };
    }, []);

    // Function to send a chat command via the data channel.
    const startChat = () => {
        if (dataChannelRef.current && dataChannelRef.current.readyState === 'open') {
            // This event instructs the API to begin a voice conversation.
            const chatEvent = {
                type: 'response.create',
                response: {
                    modalities: ['audio'], // using audio modality for voice conversation
                    instructions: 'Start voice conversation',
                },
            };
            dataChannelRef.current.send(JSON.stringify(chatEvent));
            console.log('Chat event sent:', chatEvent);
        } else {
            console.error('Data channel is not open yet.');
        }
    };

    return (
        <div>
            <h1>Realtime Chat with WebRTC</h1>
            {isConnected ? (
                <button onClick={startChat}>Start Chat</button>
            ) : (
                <p>Connecting to realtime API...</p>
            )}
            {/* Remote audio from the API will play in this audio element */}
            <audio ref={audioRef} autoPlay controls />
        </div>
    );
};

export default RealtimeChat;
