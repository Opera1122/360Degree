/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;

namespace InfinityCode.uPano.Attributes
{
    public class WizardCreateMethodAttribute : Attribute
    {
        public string methodName;

        public WizardCreateMethodAttribute(string methodName)
        {
            this.methodName = methodName;
        }
    }
}