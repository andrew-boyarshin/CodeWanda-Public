using System;
using JetBrains.Annotations;

namespace CodeWanda.Model.Graph.Nodes.Local.DataFlow.Context
{
    public static class ContextUtilities
    {
        public static VariableValueVariantBase Clone([CanBeNull] this VariableValueVariantBase self,
            [NotNull] AnalysisContext context)
        {
            switch (self)
            {
                case null:
                    return null;
                case ArrayVariant arrayVariant:
                    return arrayVariant.Clone(context);
                case CombinedVariant combinedVariant:
                    return combinedVariant.CreateNewInstance(context);
                case VariableVariant variableVariant:
                    return variableVariant.Clone(context);
                default:
                    throw new ArgumentOutOfRangeException(nameof(self));
            }
        }

        public static ValueVariantBase Clone([CanBeNull] this VariableValueVariantBase self,
            [NotNull] AnalysisContext context,
            GraphNode[] path)
        {
            var clone = self.Clone(context);
            if (clone != null)
            {
                clone.Path = path;
            }

            return clone;
        }

        public static ItemValueVariantBase Clone([CanBeNull] this ItemValueVariantBase self,
            [NotNull] AnalysisContext context)
        {
            switch (self)
            {
                case null:
                    return null;
                case ArrayItemVariant arrayItemVariant:
                    return arrayItemVariant.Clone(context);
                case CombinedItemVariant combinedItemVariant:
                    return combinedItemVariant.CreateNewInstance(context);
                default:
                    throw new ArgumentOutOfRangeException(nameof(self));
            }
        }

        public static ItemValueVariantBase Clone([CanBeNull] this ItemValueVariantBase self,
            [NotNull] AnalysisContext context,
            GraphNode[] path)
        {
            var clone = self.Clone(context);
            if (clone != null)
            {
                clone.Path = path;
            }

            return clone;
        }

        public static ValueVariantBase Clone([CanBeNull] this ValueVariantBase self, [NotNull] AnalysisContext context)
        {
            switch (self)
            {
                case null:
                    return null;
                case ItemValueVariantBase itemValueVariantBase:
                    return itemValueVariantBase.Clone(context);
                case VariableValueVariantBase variableValueVariantBase:
                    return variableValueVariantBase.Clone(context);
                default:
                    throw new ArgumentOutOfRangeException(nameof(self));
            }
        }

        public static ValueVariantBase Clone([CanBeNull] this ValueVariantBase self, [NotNull] AnalysisContext context,
            GraphNode[] path)
        {
            var clone = self.Clone(context);
            if (clone != null)
            {
                clone.Path = path;
            }

            return clone;
        }
    }
}