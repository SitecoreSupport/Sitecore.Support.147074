
namespace Sitecore.Support.ContentSearch.Linq.Lucene
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Diagnostics;
    using Sitecore.ContentSearch;
    using Sitecore.ContentSearch.Linq.Common;
    using Sitecore.ContentSearch.Linq.Lucene;

    public partial class LuceneQueryMapper
    {
        public interface IExecutionContextsModifier
        {
            void Add(IExecutionContext context);

            bool Remove(IExecutionContext context);

            IExecutionContext ReplaceFirstMatched(IExecutionContext context);
        }

        protected class ExecutionContextsModifier : IExecutionContextsModifier
        {

            protected Action<IExecutionContext[]> flushMethod;
            protected LuceneIndexParameters source;

            public ExecutionContextsModifier(LuceneIndexParameters source)
            {
                Assert.ArgumentNotNull(source, "source");
                this.source = source;
                this.flushMethod = this.GetExecutionContextsSetter(this.source);
                Assert.IsNotNull(this.flushMethod, "flushMethod can't be null");
            }

            protected Action<IExecutionContext[]> GetExecutionContextsSetter(LuceneIndexParameters source)
            {
                var property = typeof(LuceneIndexParameters).GetProperty("ExecutionContexts",
                    BindingFlags.Instance | BindingFlags.Public);
                return
                    property.SetMethod.CreateDelegate(typeof(Action<IExecutionContext[]>), source) as
                        Action<IExecutionContext[]>;
            }

            public void Add(IExecutionContext context)
            {
                Assert.ArgumentNotNull(context, "context");
                var executionContexts = this.source.ExecutionContexts;

                var updatedExecutionContexts = new IExecutionContext[executionContexts.Length + 1];

                executionContexts.CopyTo(updatedExecutionContexts, 1);
                updatedExecutionContexts[0] = context;

                this.flushMethod(updatedExecutionContexts);
            }

            public bool Remove(IExecutionContext context)
            {
                Assert.ArgumentNotNull(context, "context");
                var executionContexts = this.source.ExecutionContexts;

                var updatedExecutionContexts = executionContexts.Where(c => !Equals(c, context)).ToArray();

                var modified = executionContexts.Length != updatedExecutionContexts.Length;

                if (modified)
                {
                    this.flushMethod(updatedExecutionContexts);
                }

                return modified;
            }

            public IExecutionContext ReplaceFirstMatched(IExecutionContext newExecutionContext)
            {
                var executionContexts = this.source.ExecutionContexts;

                Func<IExecutionContext, bool> matches;

                if (newExecutionContext is CultureExecutionContext)
                {
                    matches = (op) => op is CultureExecutionContext;
                }
                else
                {
                    var contextType = newExecutionContext.GetType();
                    matches = (op) =>
                    {
                        var curContextType = op.GetType();
                        return curContextType == contextType || curContextType.IsSubclassOf(contextType);
                    };
                }

                for (int i = 0; i < executionContexts.Length; ++i)
                {
                    if (matches(executionContexts[i]))
                    {
                        var prevContext = executionContexts[i];
                        executionContexts[i] = newExecutionContext;
                        return prevContext;
                    }
                }

                return null;
            }
        }
    }
}