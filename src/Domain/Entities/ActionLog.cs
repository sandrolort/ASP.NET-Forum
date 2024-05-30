using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Enums;

namespace Domain.Entities;

// ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
public class ActionLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public uint Id { get; set; }
    public DateTime Date { get; set; }
    public string ItemType { get; set; }
    public string ItemId { get; set; }
    public OperationType OperationType { get; set; }
    public string ColumnName { get; set; }
    public string OldResult { get; set; }
    public string NewResult { get; set; }

    public ActionLog(DateTime date, string itemType, string itemId, OperationType operationType, string columnName, string oldResult, string newResult)
    {
        Date = date;
        ItemType = itemType;
        ItemId = itemId;
        OperationType = operationType;
        ColumnName = columnName;
        OldResult = oldResult;
        NewResult = newResult;
    }

    public override string ToString()
    {
        return $"Date: {Date}, ItemType: {ItemType}, ItemId: {ItemId}, OperationType: {OperationType}, ColumnName: {ColumnName}, OldResult: {OldResult}, NewResult: {NewResult}";
    }
}