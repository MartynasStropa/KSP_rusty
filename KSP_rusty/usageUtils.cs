using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KSP_rusty
{
    static class usageUtils
    {
        public const float GLOBAL_MOD = 1000.0f;

        // amount of condition used up in this frame
        public static float getPartUsageInTime(float rate) {
            // wiki.kerbalspaceprogram.com/wiki/Time
            // Kerbin year: 2556.50 hours
            // Kerbin year: 426.08 days
            // Kerbin year: 66.23 months
            const float YEAR = 2556.5f * 3600.0f;
            const float PART_LIFETIME = 1 * YEAR;

            // decrease condition based on:
            // time (all parts last 20 kerbal years: 184068000 seconds)
            return (100.0f / PART_LIFETIME) * rate;
        }
    }
}
