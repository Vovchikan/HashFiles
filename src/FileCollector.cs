using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashFiles.src
{
    interface FileCollector
    {
        void SetStash(MyConcurrentQueue<string> stash);
        void CollectFrom(params string[] paths);
    }
}
