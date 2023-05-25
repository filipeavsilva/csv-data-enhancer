using NodaTime;

namespace DataRetrieval.DTOs;

public record GleifResponseDto<T>(GleifMetadata Meta, List<T> Data);
public record GleifMetadata(GleifDataPublishDate GoldenCopy, GleifPaginationInfo Pagination);
public record GleifDataPublishDate(DateTime PublishDate);
public record GleifPaginationInfo(int CurrentPage, int PerPage, int From, int To, int Total, int LastPage);