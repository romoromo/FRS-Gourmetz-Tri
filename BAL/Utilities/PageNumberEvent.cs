using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Utilities
{
    public class PageNumberEvent : IPdfPageEvent
    {
        public void OnChapter(PdfWriter writer, Document document, float paragraphPosition, Paragraph title)
        {
            //throw new NotImplementedException();
        }

        public void OnChapterEnd(PdfWriter writer, Document document, float paragraphPosition)
        {
            //throw new NotImplementedException();
        }

        public void OnCloseDocument(PdfWriter writer, Document document)
        {
            //throw new NotImplementedException();
        }

        public void OnEndPage(PdfWriter writer, Document document)
        {
            PdfContentByte cb = writer.DirectContent;
            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

            cb.BeginText();
            cb.SetFontAndSize(bf, 10);
            cb.SetTextMatrix(document.PageSize.GetRight(50), document.PageSize.GetBottom(30));
            cb.ShowText("Page " + writer.PageNumber);
            cb.EndText();
        }

        public void OnGenericTag(PdfWriter writer, Document document, Rectangle rect, string text)
        {
            //throw new NotImplementedException();
        }

        public void OnOpenDocument(PdfWriter writer, Document document)
        {
            //throw new NotImplementedException();
        }

        public void OnParagraph(PdfWriter writer, Document document, float paragraphPosition)
        {
            //throw new NotImplementedException();
        }

        public void OnParagraphEnd(PdfWriter writer, Document document, float paragraphPosition)
        {
            //throw new NotImplementedException();
        }

        public void OnSection(PdfWriter writer, Document document, float paragraphPosition, int depth, Paragraph title)
        {
            //throw new NotImplementedException();
        }

        public void OnSectionEnd(PdfWriter writer, Document document, float paragraphPosition)
        {
            //throw new NotImplementedException();
        }

        public void OnStartPage(PdfWriter writer, Document document)
        {
            //throw new NotImplementedException();
        }
    }
}
