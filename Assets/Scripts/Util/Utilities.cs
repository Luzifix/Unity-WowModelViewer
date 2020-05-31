using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Util
{
    public static class Utilities
    {
        public static List<string> GetLocalBranch(string filepath)
        {
            var branchList = new List<string>();

            using (var reader = new StreamReader(filepath))
            {
                // Read 1 useless files.
                reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var lineSplit = line.Split(new [] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                    var branch = lineSplit.Last();
                    branchList.Add(branch);
                }
            }

            return branchList;
        }
    }
}