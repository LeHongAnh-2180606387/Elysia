using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    // Lớp này đại diện cho một đối tượng có thể tương tác trong thế giới trò chơi.
    // Nó kế thừa từ lớp WorldInteraction, và cụ thể là thực hiện hành động khi tương tác với đối tượng.
    public class InteractObject : WorldInteraction
    {
        public GameObject obj; // Đối tượng game mà chúng ta muốn tương tác với.

        // Phương thức này được gọi khi thực hiện tương tác với đối tượng.
        // Nó kích hoạt đối tượng game được chỉ định và sau đó gọi phương thức cơ sở để thực hiện các hành động tương tác khác.
        public override void InteractActual()
        {
            obj.SetActive(true); // Kích hoạt đối tượng game để nó có thể xuất hiện trong trò chơi.
            base.InteractActual(); // Gọi phương thức InteractActual() của lớp cơ sở để thực hiện các hành động tương tác khác.
        }
    }
}
