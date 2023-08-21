﻿/* 
 * Copyright (c) 2023, NCQA and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the BSD 3-Clause license
 * available at https://raw.githubusercontent.com/FirelyTeam/cql-sdk/main/LICENSE
 */

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Hl7.Cql.Compiler
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class NullConditionalMemberExpression : Expression

    {
        public MemberExpression MemberExpression { get; private set; }
        private readonly Type resultType;

        public NullConditionalMemberExpression(MemberExpression expression)
        {
            if (expression.Expression is null)
                throw new ArgumentException("Expression is not applicable to static member access");

            MemberExpression = expression;
            var isNullableType = !MemberExpression.Type.IsValueType || Nullable.GetUnderlyingType(MemberExpression.Type) is not null;
            resultType = isNullableType ? MemberExpression.Type : typeof(Nullable<>).MakeGenericType(MemberExpression.Type);
        }

        public NullConditionalMemberExpression(Expression expression, MemberInfo member) :
            this(MakeMemberAccess(expression, member))
        {
            // Nothing	
        }

        public override bool CanReduce => true;

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override Expression Reduce()
        {
            var objectVariable = Variable(MemberExpression.Expression!.Type);
            Expression notNull(Expression expression) => NotEqual(expression, Constant(null, MemberExpression.Expression.Type));

            Expression nullableMemberExpression = (MemberExpression.Type != resultType) ?
                    Convert(MemberExpression, resultType) : MemberExpression;
            var block = Block(new[] { objectVariable },
                Assign(objectVariable, MemberExpression.Expression!),
                Condition(notNull(objectVariable), nullableMemberExpression, Default(resultType)));
            return block;
        }
        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            return visitor.Visit(MemberExpression);
        }

        public override Type Type => resultType;
    }
}
