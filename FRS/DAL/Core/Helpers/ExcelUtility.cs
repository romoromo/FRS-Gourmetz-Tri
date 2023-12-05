using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace DAL.Core.Helpers
{
    public static class ExcelUtility
    {
        // Your function to get the row and check if cell is empty
        public static IRow GetRowWithNonEmptyCell(XSSFSheet sheet, int currentRowNumber, int cellNo = 0)
        {
            // Iterate from the current row number
            for (int rowIndex = currentRowNumber; rowIndex <= sheet.LastRowNum; rowIndex++)
            {
                IRow row = sheet.GetRow(rowIndex);

                if (row != null)
                {
                    // Check if cell indicated is not empty
                    ICell cell = row.GetCell(cellNo, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                    if (cell != null && cell.CellType != CellType.Blank)
                    {
                        return row;
                    }
                }
            }

            // Return null if no non-empty cell is found
            return null;
        }
    }
}
