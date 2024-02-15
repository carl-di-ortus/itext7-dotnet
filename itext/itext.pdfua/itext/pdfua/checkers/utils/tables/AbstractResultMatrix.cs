using System;
using System.Collections.Generic;
using System.Text;
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Pdfua.Exceptions;

namespace iText.Pdfua.Checkers.Utils.Tables {
    /// <summary>Class that represents a matrix of cells in a table.</summary>
    /// <remarks>
    /// Class that represents a matrix of cells in a table.
    /// It is used to check if the table has valid headers and scopes for the cells.
    /// </remarks>
    /// <typeparam name="T">The type of the cell.</typeparam>
    internal abstract class AbstractResultMatrix<T> {
        protected internal readonly ITableIterator<T> iterator;

        //We can't use an array because it is not autoportable
        private readonly IList<T> cellMatrix;

        private readonly int rows;

        private readonly int cols;

        /// <summary>
        /// Creates a new
        /// <see cref="AbstractResultMatrix{T}"/>
        /// instance.
        /// </summary>
        /// <param name="cols">The number of columns in the table.</param>
        /// <param name="iterator">The iterator that will be used to iterate over the cells.</param>
        protected internal AbstractResultMatrix(int cols, ITableIterator<T> iterator) {
            this.rows = iterator.GetAmountOfRowsHeader() + iterator.GetAmountOfRowsBody() + iterator.GetAmountOfRowsFooter
                ();
            this.cols = cols;
            this.iterator = iterator;
            cellMatrix = iText.Pdfua.Checkers.Utils.Tables.AbstractResultMatrix<T>.CreateFixedSizedList<T>(rows * cols
                , null);
        }

        /// <summary>Runs the algorithm to check if the table has valid headers and scopes for the cells.</summary>
        public virtual void CheckValidTableTagging() {
            ICollection<String> knownIds = new HashSet<String>();
            // We use boxed boolean array so we can don't duplicate our setCell methods.
            // But we fill default with false so we can avoid the null check.
            IList<bool> scopeMatrix = iText.Pdfua.Checkers.Utils.Tables.AbstractResultMatrix<T>.CreateFixedSizedList<bool
                >(rows * cols, false);
            bool hasUnknownHeaders = false;
            while (iterator.HasNext()) {
                T cell = iterator.Next();
                String role = GetRole(cell);
                int rowspan = GetRowspan(cell);
                int colspan = GetColspan(cell);
                int colIdx = GetCol(cell);
                int rowIdx = GetRow(cell);
                this.SetCell(rowIdx, rowspan, colIdx, colspan, cellMatrix, cell);
                if (StandardRoles.TH.Equals(role)) {
                    byte[] id = GetElementId(cell);
                    if (id != null) {
                        knownIds.Add(iText.Commons.Utils.JavaUtil.GetStringForBytes(id, System.Text.Encoding.UTF8));
                    }
                    String scope = GetScope(cell);
                    if (PdfName.Column.GetValue().Equals(scope)) {
                        this.SetColumnValue(colIdx, colspan, scopeMatrix, true);
                    }
                    else {
                        if (PdfName.Row.GetValue().Equals(scope)) {
                            this.SetRowValue(rowIdx, rowspan, scopeMatrix, true);
                        }
                        else {
                            if (PdfName.Both.GetValue().Equals(scope)) {
                                this.SetColumnValue(colIdx, colspan, scopeMatrix, true);
                                this.SetRowValue(rowIdx, rowspan, scopeMatrix, true);
                            }
                            else {
                                hasUnknownHeaders = true;
                            }
                        }
                    }
                }
                else {
                    if (!StandardRoles.TD.Equals(role)) {
                        String message = MessageFormatUtil.Format(PdfUAExceptionMessageConstants.CELL_HAS_INVALID_ROLE, GetNormalizedRow
                            (rowIdx), GetLocationInTable(rowIdx), colIdx);
                        throw new PdfUAConformanceException(message);
                    }
                }
            }
            ValidateTableCells(knownIds, scopeMatrix, hasUnknownHeaders);
        }

        /// <summary>Gets the colum index of the cell.</summary>
        /// <param name="cell">The cell from which the column index is needed.</param>
        /// <returns>The column index.</returns>
        internal abstract int GetCol(T cell);

        /// <summary>Gets the absolute row index of the cell including header, footer, body offset.</summary>
        /// <param name="cell">The cell from which the row index is needed.</param>
        /// <returns>The absolute row index.</returns>
        internal abstract int GetRow(T cell);

        internal virtual void SetRowValue(int row, int rowSpan, IList<bool> arr, bool value) {
            SetCell(row, rowSpan, 0, this.cols, arr, value);
        }

        internal abstract int GetRowspan(T data);

        internal abstract int GetColspan(T data);

        internal abstract IList<byte[]> GetHeaders(T cell);

        internal abstract String GetScope(T cell);

        internal abstract byte[] GetElementId(T cell);

        internal abstract String GetRole(T cell);

        private void ValidateTableCells(ICollection<String> knownIds, IList<bool> scopeMatrix, bool hasUnknownHeaders
            ) {
            StringBuilder sb = new StringBuilder();
            bool areAllTDCellsValid = true;
            for (int i = 0; i < this.cellMatrix.Count; i++) {
                T cell = this.cellMatrix[i];
                if (cell == null) {
                    String message = MessageFormatUtil.Format(PdfUAExceptionMessageConstants.TABLE_CONTAINS_EMPTY_CELLS, GetNormalizedRow
                        (i), GetLocationInTable(i), i % this.cols);
                    throw new PdfUAConformanceException(message);
                }
                String role = GetRole(cell);
                if (!StandardRoles.TD.Equals(role)) {
                    continue;
                }
                if (HasValidHeaderIds(cell, knownIds)) {
                    continue;
                }
                bool hasConnectedHeader = (bool)scopeMatrix[i];
                if (!hasConnectedHeader && hasUnknownHeaders) {
                    // we don't want to break here, we want to collect all the errors
                    areAllTDCellsValid = false;
                    int row = i / this.cols;
                    int col = i % this.cols;
                    String location = GetLocationInTable(row);
                    String message = MessageFormatUtil.Format(PdfUAExceptionMessageConstants.CELL_CANT_BE_DETERMINED_ALGORITHMICALLY
                        , GetNormalizedRow(row), col, location);
                    sb.Append(message).Append('\n');
                }
            }
            if (!areAllTDCellsValid) {
                throw new PdfUAConformanceException(sb.ToString());
            }
        }

        private String GetLocationInTable(int row) {
            if (row < iterator.GetAmountOfRowsHeader()) {
                return "Header";
            }
            else {
                if (row < iterator.GetAmountOfRowsHeader() + iterator.GetAmountOfRowsBody()) {
                    return "Body";
                }
                else {
                    return "Footer";
                }
            }
        }

        private int GetNormalizedRow(int row) {
            if (row < iterator.GetAmountOfRowsHeader()) {
                return row;
            }
            else {
                if (row < iterator.GetAmountOfRowsHeader() + iterator.GetAmountOfRowsBody()) {
                    return row - iterator.GetAmountOfRowsHeader();
                }
                else {
                    return row - iterator.GetAmountOfRowsHeader() - iterator.GetAmountOfRowsBody();
                }
            }
        }

        private void SetCell<Z>(int row, int rowSpan, int col, int colSpan, IList<Z> arr, Z value) {
            for (int i = row; i < row + rowSpan; i++) {
                for (int j = col; j < col + colSpan; j++) {
                    arr[i * this.cols + j] = value;
                }
            }
        }

        private void SetColumnValue(int col, int colSpan, IList<bool> arr, bool value) {
            SetCell(0, this.rows, col, colSpan, arr, value);
        }

        private bool HasValidHeaderIds(T cell, ICollection<String> knownIds) {
            IList<byte[]> headers = GetHeaders(cell);
            if (headers == null) {
                return false;
            }
            if (headers.IsEmpty()) {
                return false;
            }
            foreach (byte[] knownId in headers) {
                if (!knownIds.Contains(iText.Commons.Utils.JavaUtil.GetStringForBytes(knownId, System.Text.Encoding.UTF8))
                    ) {
                    return false;
                }
            }
            return true;
        }

        private static IList<Z> CreateFixedSizedList<Z>(int capacity, Object defaultValue) {
            IList<Z> arr = new List<Z>(capacity);
            for (int i = 0; i < capacity; i++) {
                arr.Add((Z)defaultValue);
            }
            return arr;
        }
    }
}
