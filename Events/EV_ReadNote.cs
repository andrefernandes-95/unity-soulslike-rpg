namespace AF
{
    using System.Collections;
    using UnityEngine;

    public class EV_ReadNote : EventBase
    {
        public Journal journal;

        UIDocumentBook uIDocumentBook;

        public override IEnumerator Dispatch()
        {
            journal.Read();

            yield return new WaitUntil(() => GetUIDocumentBook().isActiveAndEnabled == false);
        }

        UIDocumentBook GetUIDocumentBook()
        {
            if (uIDocumentBook == null) { uIDocumentBook = FindAnyObjectByType<UIDocumentBook>(FindObjectsInactive.Include); }
            return uIDocumentBook;
        }
    }

}
