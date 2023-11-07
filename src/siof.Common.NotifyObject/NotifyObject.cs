using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace siof.Common
{
    public class NotifyObject : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsDisposed { get; private set; }

        /// <summary>
        /// OnPropertyChanged catches exceptions and ignores them
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        /// OnPropertyChanged catches exceptions and ignores them
        /// </summary>
        /// <param name="expression"></param>
        protected virtual void OnPropertyChanged(Expression<Func<object>> expression)
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(GetPropertyName(expression)));
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public virtual void Dispose()
        {
            if (PropertyChanged != null)
            {
                var delegates = PropertyChanged.GetInvocationList().ToList();
                foreach (var del in delegates)
                {
                    PropertyChanged -= (PropertyChangedEventHandler)del;
                }
            }

            IsDisposed = true;
        }

        public static string GetPropertyName(Expression<Func<object>> expression)
        {
            MemberExpression memberExpression = expression.Body is UnaryExpression unaryExpression
                ? (MemberExpression)unaryExpression.Operand
                : (MemberExpression)expression.Body;

            return memberExpression.Member.Name;
        }
    }
}