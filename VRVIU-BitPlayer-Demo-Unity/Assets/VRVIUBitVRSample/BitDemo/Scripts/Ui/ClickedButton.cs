 


namespace VRVIU.LightVR.UI
{
    public class ClickedButton : BaseButton
    {
     
        protected override void Start ()
        {
            base.Start();
           
        }

        public void OnClick()
        {
            base.OnClick();
            this.ClearVRButtonState();
            if(m_selector != null)
            {
                m_selector.Hide();
            }
        }

    }
        
}