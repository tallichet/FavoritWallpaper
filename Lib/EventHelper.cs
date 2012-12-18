using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Lib
{
    public static class EventHelper
    {
        public static void Raise(this PropertyChangedEventHandler evt, INotifyPropertyChanged sender, Expression<Func<object>> expression)
        {
            if (evt != null)
            {
                if (expression.NodeType != ExpressionType.Lambda)
                {
                    throw new ArgumentException("expression must be a lamba", "expression");
                }
                MemberExpression body;
                if (expression.Body is MemberExpression)
                {
                    body = expression.Body as MemberExpression;
                }
                else if (expression.Body is UnaryExpression)
                {
                    body = (expression.Body as UnaryExpression).Operand as MemberExpression;
                }
                else
                {
                    throw new ArgumentException("your lambda expression should be a member expression", "expression");
                }

                var propertyName = body.Member.Name;

                evt(sender, new PropertyChangedEventArgs(propertyName));
            }
        }

        public static void Raise(this EventHandler evt, object sender)
        {
            if (evt != null)
            {
                evt(sender, new EventArgs());
            }
        }

        public static void Raise<T>(this EventHandler<T> handler, object sender, T args)
        {
            if (handler != null)
            {
                handler(sender, args);
            }
        }
    }
}
