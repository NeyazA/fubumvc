using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;

namespace FubuMVC.Core.View.Attachment
{
    [Policy]
    [Description("View Attachment")]
    public class ViewAttacher : IConfigurationAction
    {
        public void Configure(BehaviorGraph graph)
        {
            graph.Settings.Get<ViewEngines>().UseGraph(graph);
            var policy = graph.Settings.Get<ViewAttachmentPolicy>();

            FindLastActions(graph).Each(action => {
                policy.Profiles(graph).Each(x => {
                    Attach(policy, x.Profile, x.Views, action);
                });
            });
        }

        public virtual void Attach(ViewAttachmentPolicy policy, IViewProfile viewProfile, ViewBag bag, ActionCall action)
        {
            // No duplicate views!
            var outputNode = action.ParentChain().Output;
            if (outputNode.HasView(viewProfile.Condition)) return;


            foreach (var filter in policy.Filters())
            {
                var viewTokens = filter.Apply(action, bag);
                var count = viewTokens.Count();

                if (count != 1) continue;

                var token = viewTokens.Single().Resolve();
                outputNode.AddView(token, viewProfile.Condition);

                break;
            }
        }



        public static IEnumerable<ActionCall> FindLastActions(BehaviorGraph graph)
        {
            foreach (var chain in graph.Behaviors)
            {
                var last = chain.Calls.LastOrDefault();
                if (last != null && last.HasOutput)
                {
                    yield return last;
                }
            }
        }


    }
}