﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Singularity.Graph.Resolvers
{
    internal sealed class ExpressionDependencyResolver : IDependencyResolver
    {
        private static readonly MethodInfo GenericCreateLambdaMethod;
        static ExpressionDependencyResolver()
        {
            GenericCreateLambdaMethod = typeof(ExpressionDependencyResolver).GetMethod(nameof(CreateLambda));
        }

        public IEnumerable<ServiceBinding> Resolve(IResolverPipeline graph, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Expression<>) && type.GenericTypeArguments.Length == 1)
            {
                Type funcType = type.GenericTypeArguments[0];
                if (funcType.GetGenericTypeDefinition() == typeof(Func<>) && funcType.GenericTypeArguments.Length == 1)
                {
                    Type dependencyType = funcType.GenericTypeArguments[0];
                    MethodInfo method = GenericCreateLambdaMethod.MakeGenericMethod(dependencyType);
                    foreach (InstanceFactory instanceFactory in graph.ResolveAll(dependencyType))
                    {
                        Expression baseExpression = instanceFactory.Expression;
                        var newBinding = new ServiceBinding(type, new BindingMetadata(), baseExpression);

                        var expression = (Expression)method.Invoke(null, new object[] { baseExpression });
                        var factory = new InstanceFactory(type, expression, scoped => expression);
                        newBinding.Factories.Add(factory);
                        yield return newBinding;
                    }
                }
            }
        }

        public static LambdaExpression CreateLambda<T>(Expression expression)
        {
            return Expression.Lambda<Func<T>>(expression);
        }
    }
}