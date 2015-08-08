using UnityEngine;

namespace KSP_rusty
{
    class GUIWindow
    {
        protected Rect windowPos;
        protected int id = 0;
        protected bool visible = false;
        //protected Vector2 scrollPosition;
        protected string title = "";
        public float minWidth = 500.0f;
        public bool closeButton = true;

        public GUIWindow()
        {
            this.visible = false;
            positionToCenter();
        }
        public GUIWindow(string title)
        {
            setTitle(title);
        }

        public void positionToCenter()
        {
            if ((windowPos.x == 0) && (windowPos.y == 0))//windowPos is used to position the GUI window, lets set it in the center of the screen
            {
                windowPos = new Rect(Screen.width / 2, Screen.height / 2, 10, 10);
            }
        }

        // called by AddToPostDrawQueue
        private void OnDraw()
        {
            windowPos = GUILayout.Window(1, windowPos, drawWindowInternal, this.title, GUILayout.MinWidth(minWidth));
        }

        public void drawWindowInternal(int windowID)
        {
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            // draw close button
            if (closeButton)
            {
                if (GUILayout.Button("Close")) 
                {
                    this.hide();
                }
            }

            drawWindow(windowID);
        }
        
        // windows specific draw
        public virtual void drawWindow(int windowID)
        {
            
        }

        public void show()
        {
            if (this.visible == true) return;

            RenderingManager.AddToPostDrawQueue(3, new Callback(OnDraw));//start the GUI
            this.visible = true;
        }

        public void hide()
        {
            if (this.visible == false) return;

            RenderingManager.RemoveFromPostDrawQueue(3, new Callback(OnDraw)); //close the GUI
            this.visible = false;
        }

        public void setTitle(string title)
        {
            this.title = title;
        }
    }
}
