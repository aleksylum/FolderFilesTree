using System;
using System.Collections;
using System.Threading;

namespace FolderFilesTree
{
    public class ComponentForHash
    {
        public ComponentForHash(int numberOfFileParts, OurFile ourFileReference, CountdownEvent finished, BitArray hashCodeTemporaryResult)
        {
            NumberOfFileParts = numberOfFileParts;
            OurFileReference = ourFileReference;
            Finished = finished;
            HashCodeTemporaryResult = hashCodeTemporaryResult;
        }


        public Int32 NumberOfFileParts { get; }
        public OurFile OurFileReference { get; }
        public CountdownEvent Finished { get; }
        public BitArray HashCodeTemporaryResult { get; set; }
    }
}