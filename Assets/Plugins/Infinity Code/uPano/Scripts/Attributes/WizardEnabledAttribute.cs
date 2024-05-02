/*           INFINITY CODE           */
/*     https://infinity-code.com     */

using System;

namespace InfinityCode.uPano.Attributes
{
    public class WizardEnabledAttribute : Attribute
    {
        public bool enabled;

        public WizardEnabledAttribute(bool enabled)
        {
            this.enabled = enabled;
        }
    }
}