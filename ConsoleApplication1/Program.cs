using System;
using System.Collections.Generic;
using System.Linq;
using TFS2012.Access.ChangeSets;

namespace ConsoleApplication1
{
    class Program
    {
        private static void Main(string[] args)
        {
            var workItems = new List<int>()
            {
                27400,
                27389,
                31417,
                27439
            };
            
            Uri tfsUri = new Uri("http://tfs:8080/tfs/DefaultCollection");

            var list = ChangeSets.QueryTfsForChangeSets(tfsUri, workItems); 

            // dump results into file
            var file = System.IO.File.CreateText("output.csv");

            foreach (var changeSet in list.OrderBy(x => x.ChangesetId))
            {
                string changeSetRecord = string.Format("{0}, Committer,{2},Comment,{1},",
                    changeSet.ChangesetId,
                    changeSet.Comment,
                    changeSet.CommitterDisplayName);

                file.WriteLine(changeSetRecord);
            }

            file.Flush();
        }
    }
}
