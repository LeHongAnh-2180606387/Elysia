using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    // Lớp chứa các hàm tĩnh để tính toán các giá trị thống kê.
    public static class StatsCalculations
    {
        // Tính toán sát thương cơ bản dựa trên các thuộc tính của WeaponStats và CharacterStats.
        public static int CalculateBaseDamage(WeaponStats w, CharacterStats st, float multiplier = 1)
        {

            // Tính toán sát thương dựa trên các loại sát thương.
            float physical = (w.physical * multiplier) - st.physical;
            float strike = (w.strike * multiplier) - st.vs_strike;
            float slash = (w.slash * multiplier) - st.vs_slash;
            float thrust = (w.thrust * multiplier) - st.vs_thrust;

            // Tổng hợp sát thương vật lý.
            float sum = physical + strike + slash + thrust;

            // Tính toán sát thương dựa trên các loại yếu tố khác.
            float magic = (w.magic * multiplier) - st.magic;
            float fire = (w.fire * multiplier) - st.fire;
            float lightning = (w.lightning * multiplier) - st.lightning;
            float dark = (w.dark * multiplier) - st.dark;

            // Tổng hợp tất cả các loại sát thương.
            sum += magic + fire + lightning + dark;

            // Đảm bảo rằng sát thương không dưới 1.
            if (sum <= 0)
                sum = 1;

            // Trả về giá trị sát thương cơ bản đã làm tròn.
            return Mathf.RoundToInt(sum);
        }
    }
}
