using Common.Enums;

namespace Common.DTOs;

public record ActionLogResponse(
    int Id, 
    DateTime Date, 
    string ItemType, 
    int ItemId, 
    OperationType OperationType, 
    string ColumnName, 
    string OldResult, 
    string NewResult
    );