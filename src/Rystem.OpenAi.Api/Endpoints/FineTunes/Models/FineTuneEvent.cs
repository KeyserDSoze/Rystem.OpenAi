using System.Text.Json.Serialization;

namespace Rystem.OpenAi.FineTune
{
    public sealed class FineTuneEvent
    {
        [JsonPropertyName("object")]
        public string? Object { get; set; }
        [JsonPropertyName("created_at")]
        public int CreatedAt { get; set; }
        [JsonPropertyName("level")]
        public string? Level { get; set; }
        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }

    {
//  "data": [
//    {
//      "hyperparams": {
//        "compute_classification_metrics": false,
//        "batch_size": 1,
//        "learning_rate_multiplier": 1,
//        "n_epochs": 1,
//        "prompt_loss_weight": 0
//      },
//      "model": "ada",
//      "fine_tuned_model": "ada.ft-a1bef63f7c244db7b4a5a10a6c6c528f",
//      "training_files": [
//        {
//          "statistics": {
//            "tokens": 300,
//            "examples": 75
//          },
//          "purpose": "fine-tune",
//          "filename": "data-test-file.jsonl",
//          "id": "file-b11c397a771246118edd95d6778a2f46",
//          "status": "succeeded",
//          "created_at": 1682452713,
//          "updated_at": 1682452722,
//          "object": "file"
//        }
//      ],
//      "validation_files": [
//        {
//          "statistics": {
//            "tokens": 300,
//            "examples": 75
//          },
//          "purpose": "fine-tune",
//          "filename": "data-test-file.jsonl",
//          "id": "file-2460550761e947a3ab8c7db461f2b056",
//          "status": "succeeded",
//          "created_at": 1682452713,
//          "updated_at": 1682452722,
//          "object": "file"
//        }
//      ],
//      "result_files": [
//        {
//          "statistics": {
//            "tokens": 0,
//            "examples": 0
//          },
//          "bytes": 3821,
//          "purpose": "fine-tune-results",
//          "filename": "results.csv",
//          "id": "file-07d92352b07246f7a51592e10cba0835",
//          "status": "succeeded",
//          "created_at": 1682453340,
//          "updated_at": 1682454457,
//          "object": "file"
//        }
//      ],
//      "events": [
//        {
//          "created_at": 1682452729,
//          "level": "info",
//          "message": "Job enqueued. Waiting for jobs ahead to complete.",
//          "object": "fine-tune-event"
//        },
//        {
//    "created_at": 1682452756,
//          "level": "info",
//          "message": "Job started.",
//          "object": "fine-tune-event"
//        },
//        {
//    "created_at": 1682452803,
//          "level": "info",
//          "message": "Preprocessing started.",
//          "object": "fine-tune-event"
//        },
//        {
//    "created_at": 1682453157,
//          "level": "info",
//          "message": "Training started.",
//          "object": "fine-tune-event"
//        },
//        {
//    "created_at": 1682453340,
//          "level": "info",
//          "message": "Created results file: file-07d92352b07246f7a51592e10cba0835",
//          "object": "fine-tune-event"
//        },
//        {
//    "created_at": 1682453570,
//          "level": "info",
//          "message": "Postprocessing started.",
//          "object": "fine-tune-event"
//        },
//        {
//    "created_at": 1682454427,
//          "level": "info",
//          "message": "Job succeeded.",
//          "object": "fine-tune-event"
//        },
//        {
//    "created_at": 1682454457,
//          "level": "info",
//          "message": "Completed results file: file-07d92352b07246f7a51592e10cba0835",
//          "object": "fine-tune-event"
//        },
//        {
//    "created_at": 1682454457,
//          "level": "info",
//          "message": "Training hours billed: 0.250",
//          "object": "fine-tune-event"
//        }
//      ],
//      "id": "ft-a1bef63f7c244db7b4a5a10a6c6c528f",
//      "status": "succeeded",
//      "created_at": 1682452729,
//      "updated_at": 1682454457,
//      "object": "fine-tune"
//    },
//    {
//    "hyperparams": {
//        "compute_classification_metrics": false,
//        "batch_size": 1,
//        "learning_rate_multiplier": 1,
//        "n_epochs": 1,
//        "prompt_loss_weight": 0
//      },
//      "model": "ada",
//      "fine_tuned_model": "ada.ft-6ca820d26b0346fd8f6d743b06fcacd9",
//      "training_files": [
//        {
//        "statistics": {
//            "tokens": 300,
//            "examples": 75
//          },
//          "purpose": "fine-tune",
//          "filename": "data-test-file.jsonl",
//          "id": "file-9709565529dd46fa83d70691a0fcfc3c",
//          "status": "succeeded",
//          "created_at": 1682959111,
//          "updated_at": 1682959122,
//          "object": "file"
//        }
//      ],
//      "validation_files": [
//        {
//        "statistics": {
//            "tokens": 300,
//            "examples": 75
//          },
//          "purpose": "fine-tune",
//          "filename": "data-test-file.jsonl",
//          "id": "file-5206e13457434748adc7b7c6cc8559f7",
//          "status": "succeeded",
//          "created_at": 1682959112,
//          "updated_at": 1682959122,
//          "object": "file"
//        }
//      ],
//      "result_files": [
//        {
//        "statistics": {
//            "tokens": 0,
//            "examples": 0
//          },
//          "bytes": 3945,
//          "purpose": "fine-tune-results",
//          "filename": "results.csv",
//          "id": "file-1c7c147217bf49819e945b278d4ae63d",
//          "status": "succeeded",
//          "created_at": 1682959789,
//          "updated_at": 1682960793,
//          "object": "file"
//        }
//      ],
//      "events": [
//        {
//        "created_at": 1682959127,
//          "level": "info",
//          "message": "Job enqueued. Waiting for jobs ahead to complete.",
//          "object": "fine-tune-event"
//        },
//        {
//        "created_at": 1682959204,
//          "level": "info",
//          "message": "Job started.",
//          "object": "fine-tune-event"
//        },
//        {
//        "created_at": 1682959256,
//          "level": "info",
//          "message": "Preprocessing started.",
//          "object": "fine-tune-event"
//        },
//        {
//        "created_at": 1682959609,
//          "level": "info",
//          "message": "Training started.",
//          "object": "fine-tune-event"
//        },
//        {
//        "created_at": 1682959789,
//          "level": "info",
//          "message": "Created results file: file-1c7c147217bf49819e945b278d4ae63d",
//          "object": "fine-tune-event"
//        },
//        {
//        "created_at": 1682960024,
//          "level": "info",
//          "message": "Postprocessing started.",
//          "object": "fine-tune-event"
//        },
//        {
//        "created_at": 1682960738,
//          "level": "info",
//          "message": "Job succeeded.",
//          "object": "fine-tune-event"
//        },
//        {
//        "created_at": 1682960793,
//          "level": "info",
//          "message": "Completed results file: file-1c7c147217bf49819e945b278d4ae63d",
//          "object": "fine-tune-event"
//        },
//        {
//        "created_at": 1682960793,
//          "level": "info",
//          "message": "Training hours billed: 0.250",
//          "object": "fine-tune-event"
//        }
//      ],
//      "id": "ft-6ca820d26b0346fd8f6d743b06fcacd9",
//      "status": "succeeded",
//      "created_at": 1682959127,
//      "updated_at": 1682960793,
//      "object": "fine-tune"
//    },
//    {
//    "hyperparams": {
//        "compute_classification_metrics": false,
//        "batch_size": 1,
//        "learning_rate_multiplier": 1,
//        "n_epochs": 1,
//        "prompt_loss_weight": 0
//      },
//      "model": "ada",
//      "fine_tuned_model": "ada.ft-8d64d466529a4234943a55aaa295f0b1",
//      "training_files": [
//        {
//        "statistics": {
//            "tokens": 300,
//            "examples": 75
//          },
//          "purpose": "fine-tune",
//          "filename": "data-test-file.jsonl",
//          "id": "file-f95428f7775d4d8395054b272371e41c",
//          "status": "succeeded",
//          "created_at": 1682977516,
//          "updated_at": 1682977525,
//          "object": "file"
//        }
//      ],
//      "validation_files": [
//        {
//        "statistics": {
//            "tokens": 300,
//            "examples": 75
//          },
//          "purpose": "fine-tune",
//          "filename": "data-test-file.jsonl",
//          "id": "file-d5ac7c49a09246eeb1aaacd8c8eaad9c",
//          "status": "succeeded",
//          "created_at": 1682977516,
//          "updated_at": 1682977525,
//          "object": "file"
//        }
//      ],
//      "result_files": [
//        {
//        "statistics": {
//            "tokens": 0,
//            "examples": 0
//          },
//          "bytes": 3887,
//          "purpose": "fine-tune-results",
//          "filename": "results.csv",
//          "id": "file-0475740456324518bc872bb0ffbc74da",
//          "status": "succeeded",
//          "created_at": 1682978091,
//          "updated_at": 1682979260,
//          "object": "file"
//        }
//      ],
//      "events": [
//        {
//        "created_at": 1682977532,
//          "level": "info",
//          "message": "Job enqueued. Waiting for jobs ahead to complete.",
//          "object": "fine-tune-event"
//        },
//        {
//        "created_at": 1682977553,
//          "level": "info",
//          "message": "Job started.",
//          "object": "fine-tune-event"
//        },
//        {
//        "created_at": 1682977604,
//          "level": "info",
//          "message": "Preprocessing started.",
//          "object": "fine-tune-event"
//        },
//        {
//        "created_at": 1682977958,
//          "level": "info",
//          "message": "Training started.",
//          "object": "fine-tune-event"
//        },
//        {
//        "created_at": 1682978091,
//          "level": "info",
//          "message": "Created results file: file-0475740456324518bc872bb0ffbc74da",
//          "object": "fine-tune-event"
//        },
//        {
//        "created_at": 1682978433,
//          "level": "info",
//          "message": "Postprocessing started.",
//          "object": "fine-tune-event"
//        },
//        {
//        "created_at": 1682979212,
//          "level": "info",
//          "message": "Job succeeded.",
//          "object": "fine-tune-event"
//        },
//        {
//        "created_at": 1682979260,
//          "level": "info",
//          "message": "Completed results file: file-0475740456324518bc872bb0ffbc74da",
//          "object": "fine-tune-event"
//        },
//        {
//        "created_at": 1682979260,
//          "level": "info",
//          "message": "Training hours billed: 0.250",
//          "object": "fine-tune-event"
//        }
//      ],
//      "id": "ft-8d64d466529a4234943a55aaa295f0b1",
//      "status": "succeeded",
//      "created_at": 1682977532,
//      "updated_at": 1682979260,
//      "object": "fine-tune"
//    },
//    {
//    "hyperparams": {
//        "compute_classification_metrics": false,
//        "batch_size": 1,
//        "learning_rate_multiplier": 1,
//        "n_epochs": 1,
//        "prompt_loss_weight": 0
//      },
//      "model": "ada",
//      "fine_tuned_model": "ada.ft-4abe7559721a416497e4e1646cb518ff",
//      "training_files": [
//        {
//        "statistics": {
//            "tokens": 300,
//            "examples": 75
//          },
//          "purpose": "fine-tune",
//          "filename": "data-test-file.jsonl",
//          "id": "file-4298d8f1910b4a198c08ce5b43ab7cbd",
//          "status": "succeeded",
//          "created_at": 1682979990,
//          "updated_at": 1682979995,
//          "object": "file"
//        }
//      ],
//      "validation_files": [
//        {
//        "statistics": {
//            "tokens": 300,
//            "examples": 75
//          },
//          "purpose": "fine-tune",
//          "filename": "data-test-file.jsonl",
//          "id": "file-cf11ba88703e4a1fb9355a8180ba8cc1",
//          "status": "succeeded",
//          "created_at": 1682979990,
//          "updated_at": 1682979995,
//          "object": "file"
//        }
//      ],
//      "result_files": [
//        {
//        "statistics": {
//            "tokens": 0,
//            "examples": 0
//          },
//          "bytes": 3819,
//          "purpose": "fine-tune-results",
//          "filename": "results.csv",
//          "id": "file-bb22027c52224072b53423f029f02513",
//          "status": "succeeded",
//          "created_at": 1682980680,
//          "updated_at": 1682981855,
//          "object": "file"
//        }
//      ],
//      "events": [
//        {
//        "created_at": 1682980006,
//          "level": "info",
//          "message": "Job enqueued. Waiting for jobs ahead to complete.",
//          "object": "fine-tune-event"
//        },
//        {
//        "created_at": 1682980030,
//          "level": "info",
//          "message": "Job started.",
//          "object": "fine-tune-event"
//        },
//        {
//        "created_at": 1682980085,
//          "level": "info",
//          "message": "Preprocessing started.",
//          "object": "fine-tune-event"
//        },
//        {
//        "created_at": 1682980497,
//          "level": "info",
//          "message": "Training started.",
//          "object": "fine-tune-event"
//        },
//        {
//        "created_at": 1682980680,
//          "level": "info",
//          "message": "Created results file: file-bb22027c52224072b53423f029f02513",
//          "object": "fine-tune-event"
//        },
//        {
//        "created_at": 1682980974,
//          "level": "info",
//          "message": "Postprocessing started.",
//          "object": "fine-tune-event"
//        },
//        {
//        "created_at": 1682981820,
//          "level": "info",
//          "message": "Job succeeded.",
//          "object": "fine-tune-event"
//        },
//        {
//        "created_at": 1682981855,
//          "level": "info",
//          "message": "Completed results file: file-bb22027c52224072b53423f029f02513",
//          "object": "fine-tune-event"
//        },
//        {
//        "created_at": 1682981855,
//          "level": "info",
//          "message": "Training hours billed: 0.250",
//          "object": "fine-tune-event"
//        }
//      ],
//      "id": "ft-4abe7559721a416497e4e1646cb518ff",
//      "status": "succeeded",
//      "created_at": 1682980006,
//      "updated_at": 1682981855,
//      "object": "fine-tune"
//    },
//    {
//    "hyperparams": {
//        "compute_classification_metrics": false,
//        "batch_size": 1,
//        "learning_rate_multiplier": 1,
//        "n_epochs": 1,
//        "prompt_loss_weight": 0
//      },
//      "model": "ada",
//      "training_files": [
//        {
//        "statistics": {
//            "tokens": 300,
//            "examples": 75
//          },
//          "purpose": "fine-tune",
//          "filename": "WBa3CEztLHMulfz1Z3qPak9AENcw2i4CBk85H7cfP\u002BgOuPmM7c5ZM/lpQ\u002B17nnJo",
//          "id": "file-d3cda30b3ac94883bb9d5f1019777427",
//          "status": "succeeded",
//          "created_at": 1685868694,
//          "updated_at": 1685868702,
//          "object": "file"
//        }
//      ],
//      "validation_files": [
//        {
//        "statistics": {
//            "tokens": 300,
//            "examples": 75
//          },
//          "purpose": "fine-tune",
//          "filename": "kfQZGBMs4R8YIM6bwTYBqeaQzjwhCTiWsmXw3TdSAt1ZIjrLU0\u002B6x76xc2U/RBXl",
//          "id": "file-94a2ea7f0b8a4dc2ad24ccf97288fe6d",
//          "status": "succeeded",
//          "created_at": 1685868696,
//          "updated_at": 1685868702,
//          "object": "file"
//        }
//      ],
//      "result_files": [
//        {
//        "bytes": 3832,
//          "purpose": "fine-tune-results",
//          "filename": "results.csv",
//          "id": "file-285935557aad45dfbfa0ef351c643a10",
//          "status": "running",
//          "created_at": 1685869389,
//          "updated_at": 1685869389,
//          "object": "file"
//        }
//      ],
//      "events": [
//        {
//        "created_at": 1685868736,
//          "level": "info",
//          "message": "Job enqueued. Waiting for jobs ahead to complete.",
//          "object": "fine-tune-event"
//        },
//        {
//        "created_at": 1685868804,
//          "level": "info",
//          "message": "Job started.",
//          "object": "fine-tune-event"
//        },
//        {
//        "created_at": 1685868861,
//          "level": "info",
//          "message": "Preprocessing started.",
//          "object": "fine-tune-event"
//        },
//        {
//        "created_at": 1685869208,
//          "level": "info",
//          "message": "Training started.",
//          "object": "fine-tune-event"
//        },
//        {
//        "created_at": 1685869389,
//          "level": "info",
//          "message": "Created results file: file-285935557aad45dfbfa0ef351c643a10",
//          "object": "fine-tune-event"
//        },
//        {
//        "created_at": 1685869682,
//          "level": "info",
//          "message": "Postprocessing started.",
//          "object": "fine-tune-event"
//        }
//      ],
//      "id": "ft-b10adb3bba6d4d60a166e5a2ecb52e32",
//      "status": "failed",
//      "created_at": 1685868736,
//      "updated_at": 1685869781,
//      "object": "fine-tune"
//    }
//  ],
//  "object": "list"
//}

}
