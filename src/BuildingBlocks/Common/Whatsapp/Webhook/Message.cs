using System.Text.Json.Serialization;

namespace Whatsapp.Flow.BuildingBlocks.Common.Whatsapp.Webhook
{
    public class Message
    {
        [JsonPropertyName("from")]
        public required string From { get; set; }

        [JsonPropertyName("id")]
        public required string Id { get; set; }

        [JsonPropertyName("timestamp")]
        public required string Timestamp { get; set; }

        [JsonPropertyName("type")]
        public required string Type { get; set; } // "button_reply" veya "list_reply"

        [JsonPropertyName("text")]
        public Text Text { get; set; }
        
        [JsonPropertyName("interactive")]
        public Interactive Interactive { get; set; }

        [JsonPropertyName("image")]
        public Image Image { get; set; }

        [JsonPropertyName("video")]
        public Video Video { get; set; }

        [JsonPropertyName("context")]
        public Context Context { get; set; }

        [JsonPropertyName("referral")]
        public Referral Referral { get; set; }
    }

    public class Text
    {
        [JsonPropertyName("body")]
        public required string Body { get; set; }
    }
    
    public class Interactive
    {
        [JsonPropertyName("type")]
        public required string Type { get; set; } // "button_reply" veya "list_reply"

        [JsonPropertyName("button_reply")]
        public ButtonReply ButtonReply { get; set; }

        [JsonPropertyName("list_reply")]
        public ListReply ListReply { get; set; }
    }

    public class ButtonReply
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        [JsonPropertyName("title")]
        public required string Title { get; set; }
    }

    public class ListReply
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        [JsonPropertyName("title")]
        public required string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }

    public class Image
    {
        [JsonPropertyName("caption")]
        public required string Caption { get; set; }

        [JsonPropertyName("mime_type")]
        public required string MimeType { get; set; }

        [JsonPropertyName("sha256")]
        public required string Sha256 { get; set; }

        [JsonPropertyName("id")]
        public required string Id { get; set; }
    }

    public class Video
    {
        [JsonPropertyName("caption")]
        public required string Caption { get; set; }

        [JsonPropertyName("mime_type")]
        public required string MimeType { get; set; }

        [JsonPropertyName("sha256")]
        public required string Sha256 { get; set; }

        [JsonPropertyName("id")]
        public required string Id { get; set; }
    }

    public class Context
    {
        [JsonPropertyName("forwarded")]
        public bool Forwarded { get; set; }

        [JsonPropertyName("frequently_forwarded")]
        public bool FrequentlyForwarded { get; set; }
    }

    public class Referral
    {
        [JsonPropertyName("source_url")]
        public required string SourceUrl { get; set; }

        [JsonPropertyName("source_id")]
        public required string SourceId { get; set; }

        [JsonPropertyName("source_type")]
        public required string SourceType { get; set; }

        [JsonPropertyName("body")]
        public required string Body { get; set; }

        [JsonPropertyName("headline")]
        public required string Headline { get; set; }

        [JsonPropertyName("media_type")]
        public required string MediaType { get; set; }

        [JsonPropertyName("image_url")]
        public required string ImageUrl { get; set; }

        [JsonPropertyName("video_url")]
        public required string VideoUrl { get; set; }

        [JsonPropertyName("thumbnail_url")]
        public required string ThumbnailUrl { get; set; }

        [JsonPropertyName("ctwa_clid")]
        public required string CtwaClid { get; set; }

        [JsonPropertyName("welcome_message")]
        public WelcomeMessage WelcomeMessage { get; set; }
    }

    public class WelcomeMessage
    {
        [JsonPropertyName("text")]
        public required string Text { get; set; }
    }
} 