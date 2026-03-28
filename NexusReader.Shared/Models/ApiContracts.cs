namespace NexusReader.Shared.Models
{
    public record CreateBookRequest(
        string Title,
        string Author,
        string Description,
        string? CoverImageUrl,
        string Category,
        string ColorTheme);

    public record UpdateBookRequest(
        string Title,
        string Author,
        string Description,
        string? CoverImageUrl,
        string Category,
        string ColorTheme,
        int Progress);

    public record UpsertChapterRequest(
        int? Id,
        int BookId,
        int ChapterNumber,
        string Title,
        string Content);

    public record FavoriteRequest(int BookId);

    public record BookSummaryResponse(
        int Id,
        string Title,
        string Author,
        string Category,
        string? CoverImageUrl,
        string ColorTheme,
        int Progress,
        DateTime UploadDate,
        int ChapterCount);

    public record ChapterListItemResponse(int Id, int ChapterNumber, string Title);

    public record ChapterContentResponse(
        int Id,
        int BookId,
        int ChapterNumber,
        string Title,
        string Content,
        string BookTitle,
        int? NextChapterId,
        int? PrevChapterId);
}
