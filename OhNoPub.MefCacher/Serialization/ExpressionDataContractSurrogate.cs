using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace OhNoPub.MefCacher.Serialization
{
    /// <summary>
    ///   A serializer for linq expressions.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     This implementation is incomplete and a proof of concept. Ideally
    ///     we wouldn’t need to implement this ourselves, but it seems like the
    ///     popular existing alternatives either have restrictive licenses or no
    ///     clearly endorsed nuget package. Please contribute to this implementation
    ///     to incrementally complete it. Breaking it out to its own library to
    ///     provide a clean, simple, clearly licensed nuget would probably be the
    ///     right thing to do.
    ///   </para>
    /// </remarks>
    public class ExpressionDataContractSurrogate
        : TypeReplacingDataContractSurrogate
    {
        public override IEnumerable<Type> KnownTypes => new[] {
            typeof(Expression),
            typeof(Expr),
            typeof(ExprArray),
        };

        public override Type GetDataContractType(Type type)
        {
            // Generate surrogate here if appropriate…
            if (typeof(Expression).IsAssignableFrom(type))
                return typeof(Expr);
            if (typeof(Expression[]).IsAssignableFrom(type))
                return typeof(ExprArray);
            return base.GetDataContractType(type); ;
        }

        public override object GetDeserializedObject(object obj, Type targetType)
            => (obj as Expr)?.ToExpression()
            ?? (obj as ExprArray)?.ToArray()
            ?? base.GetDeserializedObject(obj, targetType);

        public override object GetObjectToSerialize(object obj, Type targetType)
        {
            var expr = obj as Expression;
            if (expr != null)
                return Expr.FromExpression(expr);
            var exprArray = obj as Expression[];
            if (exprArray != null)
                return new ExprArray(exprArray.GetType().GetElementType(), exprArray.ToList());
            return base.GetObjectToSerialize(obj, targetType);
        }

        public override Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
        {
            if (typeName == nameof(Expr) && typeNamespace == "OhNoPub.MefCacher.Serialization")
                return typeof(Expression);
            return base.GetReferencedTypeOnImport(typeName, typeNamespace, customData);
        }

        [DataContract]
        class ExprArray
        {
            [DataMember]
            Type ElementType { get; set; }

            [DataMember]
            List<Expression> Exprs { get; set; }

            public ExprArray(
                Type elementType,
                List<Expression> exprs)
            {
                ElementType = elementType;
                Exprs = exprs;
            }

            public object ToArray()
            {
                var arr = (Expression[])Array.CreateInstance(ElementType, Exprs.Count);
                Exprs.CopyTo(arr);
                return arr;
            }
        }

        [DataContract]
        class Expr
        {
            [DataMember]
            ExpressionType Type { get; set; }
            [DataMember]
            Expression[] Exprs { get; set; }
            [DataMember]
            Expression[] PExprs { get; set; }
            [DataMember]
            ExpressionType[] ExprTypes { get; set; }
            [DataMember]
            string[] S { get; set; }
            [DataMember]
            Type[] T { get; set; }
            [DataMember]
            bool[] B { get; set; }
            [DataMember]
            object[] O { get; set; }

            Expr(
                ExpressionType type,
                Expression[] exprs = null,
                Expression[] pExprs = null,
                ExpressionType[] exprTypes = null,
                string[] s = null,
                Type[] t = null,
                bool[] b = null,
                object[] o = null)
            {
                Type = type;
                Exprs = exprs;
                PExprs = pExprs;
                ExprTypes = exprTypes;
                S = s;
                T = t;
                B = b;
                O = o;
            }

            public Expression ToExpression()
            {
                var parameterExpressions = PExprs?.Cast<ParameterExpression>().ToArray();
                switch (Type)
                {
                    case ExpressionType.Add: return Expression.Add(Exprs[0], Exprs[1]);
                    case ExpressionType.AddChecked: return Expression.AddChecked(Exprs[0], Exprs[1]);
                    case ExpressionType.And: return Expression.And(Exprs[0], Exprs[1]);
                    case ExpressionType.AndAlso:return Expression.AndAlso(Exprs[0], Exprs[1]);
                    case ExpressionType.ArrayLength:
                        break;
                    case ExpressionType.ArrayIndex:
                        break;
                    case ExpressionType.Block: return Expression.Block(T[0], parameterExpressions, Exprs);
                    case ExpressionType.Call:
                        break;
                    case ExpressionType.Coalesce: return Expression.Coalesce(Exprs[0], Exprs[1], (LambdaExpression)Exprs[2]);
                    case ExpressionType.Conditional:
                        break;
                    case ExpressionType.Constant:return Expression.Constant(O[0], T[0]);
                    case ExpressionType.Convert:
                        break;
                    case ExpressionType.ConvertChecked:
                        break;
                    case ExpressionType.Divide:
                        break;
                    case ExpressionType.Equal:
                        break;
                    case ExpressionType.ExclusiveOr:
                        break;
                    case ExpressionType.GreaterThan:
                        break;
                    case ExpressionType.GreaterThanOrEqual:
                        break;
                    case ExpressionType.Invoke:
                        break;
                        // The question is: do we have to call Expression.Lambda<T>() to get an Expression<T>
                        // or can we upcast the return value of Expression.Lambda()?
                    case ExpressionType.Lambda: return Expression.Lambda(T[0], Exprs[0], S[0], B[0], parameterExpressions);
                    case ExpressionType.LeftShift:
                        break;
                    case ExpressionType.LessThan:
                        break;
                    case ExpressionType.LessThanOrEqual:
                        break;
                    case ExpressionType.ListInit:
                        break;
                    case ExpressionType.MemberAccess:
                        break;
                    case ExpressionType.MemberInit:
                        break;
                    case ExpressionType.Modulo:
                        break;
                    case ExpressionType.Multiply:
                        break;
                    case ExpressionType.MultiplyChecked:
                        break;
                    case ExpressionType.Negate:
                        break;
                    case ExpressionType.UnaryPlus:
                        break;
                    case ExpressionType.NegateChecked:
                        break;
                    case ExpressionType.New:
                        break;
                    case ExpressionType.NewArrayInit:
                        break;
                    case ExpressionType.NewArrayBounds:
                        break;
                    case ExpressionType.Not:
                        break;
                    case ExpressionType.NotEqual:
                        break;
                    case ExpressionType.Or:
                        break;
                    case ExpressionType.OrElse:
                        break;
                    case ExpressionType.Parameter:
                        break;
                    case ExpressionType.Power:
                        break;
                    case ExpressionType.Quote:
                        break;
                    case ExpressionType.RightShift:
                        break;
                    case ExpressionType.Subtract:
                        break;
                    case ExpressionType.SubtractChecked:
                        break;
                    case ExpressionType.TypeAs:
                        break;
                    case ExpressionType.TypeIs:
                        break;
                    case ExpressionType.Assign:
                        break;
                    case ExpressionType.DebugInfo:
                        break;
                    case ExpressionType.Decrement:
                        break;
                    case ExpressionType.Dynamic:
                        break;
                    case ExpressionType.Default:
                        break;
                    case ExpressionType.Extension:
                        break;
                    case ExpressionType.Goto:
                        break;
                    case ExpressionType.Increment:
                        break;
                    case ExpressionType.Index:
                        break;
                    case ExpressionType.Label:
                        break;
                    case ExpressionType.RuntimeVariables:
                        break;
                    case ExpressionType.Loop:
                        break;
                    case ExpressionType.Switch:
                        break;
                    case ExpressionType.Throw:
                        break;
                    case ExpressionType.Try:
                        break;
                    case ExpressionType.Unbox:
                        break;
                    case ExpressionType.AddAssign:
                        break;
                    case ExpressionType.AndAssign:
                        break;
                    case ExpressionType.DivideAssign:
                        break;
                    case ExpressionType.ExclusiveOrAssign:
                        break;
                    case ExpressionType.LeftShiftAssign:
                        break;
                    case ExpressionType.ModuloAssign:
                        break;
                    case ExpressionType.MultiplyAssign:
                        break;
                    case ExpressionType.OrAssign:
                        break;
                    case ExpressionType.PowerAssign:
                        break;
                    case ExpressionType.RightShiftAssign:
                        break;
                    case ExpressionType.SubtractAssign:
                        break;
                    case ExpressionType.AddAssignChecked:
                        break;
                    case ExpressionType.MultiplyAssignChecked:
                        break;
                    case ExpressionType.SubtractAssignChecked:
                        break;
                    case ExpressionType.PreIncrementAssign:
                        break;
                    case ExpressionType.PreDecrementAssign:
                        break;
                    case ExpressionType.PostIncrementAssign:
                        break;
                    case ExpressionType.PostDecrementAssign:
                        break;
                    case ExpressionType.TypeEqual:
                        break;
                    case ExpressionType.OnesComplement:
                        break;
                    case ExpressionType.IsTrue:
                        break;
                    case ExpressionType.IsFalse:
                        break;
                }
                throw new NotImplementedException($"I do not yet know how to unserialize {Type}");
            }

            static Expr[] FromExpressions(IEnumerable<Expression> expressions)
                => expressions.Select(expression => FromExpression(expression)).ToArray();

            public static Expr FromExpression(Expression expression)
            {
                switch (expression.NodeType)
                {
                    case ExpressionType.Add:
                    case ExpressionType.AddChecked:
                    case ExpressionType.And:
                    case ExpressionType.AndAlso:
                    case ExpressionType.Divide:
                    case ExpressionType.Subtract:
                    case ExpressionType.SubtractChecked:
                        var binaryExpression = (BinaryExpression)expression;
                        if (binaryExpression.Method != null) throw new NotImplementedException("Non built-in binary expression not supported");
                        return new Expr(expression.NodeType, new[] { binaryExpression.Left, binaryExpression.Right, });
                    case ExpressionType.ArrayLength:
                        break;
                    case ExpressionType.ArrayIndex:
                        break;
                    case ExpressionType.Call:
                        break;
                    case ExpressionType.Coalesce:
                        var binaryExpression2 = (BinaryExpression)expression;
                        return new Expr(expression.NodeType, new[] { binaryExpression2.Left, binaryExpression2.Right, binaryExpression2.Conversion, });
                    case ExpressionType.Conditional:
                        break;
                    case ExpressionType.Constant:
                        var constantExpression = (ConstantExpression)expression;
                        return new Expr(expression.NodeType, t: new[] { constantExpression.Type, }, o: new[] { constantExpression.Value, });
                    case ExpressionType.Convert:
                        break;
                    case ExpressionType.ConvertChecked:
                        break;
                    case ExpressionType.Equal:
                        break;
                    case ExpressionType.ExclusiveOr:
                        break;
                    case ExpressionType.GreaterThan:
                        break;
                    case ExpressionType.GreaterThanOrEqual:
                        break;
                    case ExpressionType.Invoke:
                        break;
                    case ExpressionType.Lambda:
                        var lambdaExpression = (LambdaExpression)expression;
                        return new Expr(expression.NodeType, new[] { lambdaExpression.Body, }, lambdaExpression.Parameters.ToArray(), t: new[] { lambdaExpression.Type, }, s:new[] { lambdaExpression.Name, }, b: new[] { lambdaExpression.TailCall, });
                    case ExpressionType.LeftShift:
                        break;
                    case ExpressionType.LessThan:
                        break;
                    case ExpressionType.LessThanOrEqual:
                        break;
                    case ExpressionType.ListInit:
                        break;
                    case ExpressionType.MemberAccess:
                        break;
                    case ExpressionType.MemberInit:
                        break;
                    case ExpressionType.Modulo:
                        break;
                    case ExpressionType.Multiply:
                        break;
                    case ExpressionType.MultiplyChecked:
                        break;
                    case ExpressionType.Negate:
                        break;
                    case ExpressionType.UnaryPlus:
                        break;
                    case ExpressionType.NegateChecked:
                        break;
                    case ExpressionType.New:
                        break;
                    case ExpressionType.NewArrayInit:
                        break;
                    case ExpressionType.NewArrayBounds:
                        break;
                    case ExpressionType.Not:
                        break;
                    case ExpressionType.NotEqual:
                        break;
                    case ExpressionType.Or:
                        break;
                    case ExpressionType.OrElse:
                        break;
                    case ExpressionType.Parameter:
                        break;
                    case ExpressionType.Power:
                        break;
                    case ExpressionType.Quote:
                        break;
                    case ExpressionType.RightShift:
                        break;
                    case ExpressionType.TypeAs:
                        break;
                    case ExpressionType.TypeIs:
                        break;
                    case ExpressionType.Assign:
                        break;
                    case ExpressionType.Block:
                        var blockExpression = (BlockExpression)expression;
                        return new Expr(expression.NodeType, t: new[] { blockExpression.Type, }, pExprs: blockExpression.Variables.ToArray(), exprs: blockExpression.Expressions.ToArray());
                    case ExpressionType.DebugInfo:
                        break;
                    case ExpressionType.Decrement:
                        break;
                    case ExpressionType.Dynamic:
                        break;
                    case ExpressionType.Default:
                        break;
                    case ExpressionType.Extension:
                        break;
                    case ExpressionType.Goto:
                        break;
                    case ExpressionType.Increment:
                        break;
                    case ExpressionType.Index:
                        break;
                    case ExpressionType.Label:
                        break;
                    case ExpressionType.RuntimeVariables:
                        break;
                    case ExpressionType.Loop:
                        break;
                    case ExpressionType.Switch:
                        break;
                    case ExpressionType.Throw:
                        break;
                    case ExpressionType.Try:
                        break;
                    case ExpressionType.Unbox:
                        break;
                    case ExpressionType.AddAssign:
                        break;
                    case ExpressionType.AndAssign:
                        break;
                    case ExpressionType.DivideAssign:
                        break;
                    case ExpressionType.ExclusiveOrAssign:
                        break;
                    case ExpressionType.LeftShiftAssign:
                        break;
                    case ExpressionType.ModuloAssign:
                        break;
                    case ExpressionType.MultiplyAssign:
                        break;
                    case ExpressionType.OrAssign:
                        break;
                    case ExpressionType.PowerAssign:
                        break;
                    case ExpressionType.RightShiftAssign:
                        break;
                    case ExpressionType.SubtractAssign:
                        break;
                    case ExpressionType.AddAssignChecked:
                        break;
                    case ExpressionType.MultiplyAssignChecked:
                        break;
                    case ExpressionType.SubtractAssignChecked:
                        break;
                    case ExpressionType.PreIncrementAssign:
                        break;
                    case ExpressionType.PreDecrementAssign:
                        break;
                    case ExpressionType.PostIncrementAssign:
                        break;
                    case ExpressionType.PostDecrementAssign:
                        break;
                    case ExpressionType.TypeEqual:
                        break;
                    case ExpressionType.OnesComplement:
                        break;
                    case ExpressionType.IsTrue:
                        break;
                    case ExpressionType.IsFalse:
                        break;
                }
                throw new NotImplementedException($"I do not yet know how to serialize {expression.NodeType}");
            }
        }
    }
}
