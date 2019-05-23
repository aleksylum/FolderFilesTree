using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FolderFilesTree
{
    public class PartOfFileForHash
    {
        public PartOfFileForHash(byte[] partOfFile, ComponentForHash component)
        {
            PartOfFile = partOfFile;
            _component = component;

        }

        public Byte[] PartOfFile { get; set; }

        private ComponentForHash _component;

        public BitArray HashCodeTemporaryResult
        {
            get => _component.HashCodeTemporaryResult;
            set => _component.HashCodeTemporaryResult = value;
        }

        public OurFile OurFileReference => _component.OurFileReference;

        public CountdownEvent Finished => _component.Finished;

    }
}