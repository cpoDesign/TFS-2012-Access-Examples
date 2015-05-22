using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace TFS2012.Access.ChangeSets
{
    public class ChangeSets
    {
        public static List<Changeset> QueryTfsForChangeSets(Uri tfsUri, List<int> workItems)
        {
            var list = new List<Changeset>();
            using (var projectCollection = new TfsTeamProjectCollection(tfsUri, new UICredentialsProvider()))
            {
                projectCollection.EnsureAuthenticated();
                var workItemStore = projectCollection.GetService<WorkItemStore>();
                var versionControlServer = projectCollection.GetService<VersionControlServer>();
                var artifactProvider = versionControlServer.ArtifactProvider;
                string queryString = string.Format("Select [State], [Title] From WorkItems Where ([System.Id] In ({0}))", string.Join(",", workItems));

                // Set up a dictionary to pass "User Story" as the value of the type variable.
                Dictionary<string, string> variables = new Dictionary<string, string>();

                // Create and run the query.
                var query = new Query(workItemStore, queryString, variables);

                WorkItemCollection results = query.RunQuery();

                foreach (WorkItem workItem in results)
                {
                    //Console.WriteLine("WI: {0}, Title: {1}", workItem.Id, workItem.Title);
                    foreach (Changeset changeset in workItem.Links.OfType<ExternalLink>()
                        .Select(link => artifactProvider
                            .GetChangeset(new Uri(link.LinkedArtifactUri)))
                        .OrderBy(x => x.ChangesetId))
                    {
                        list.Add(changeset);
                    }
                }
            }

            return list;
        }
    }
}
