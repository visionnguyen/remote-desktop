﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UIControls
{
    public partial class AudioControl : UserControl
    {
        #region private members


        #endregion

        #region c-tor

        public AudioControl()
        {
            InitializeComponent();
        }

        #endregion

        #region event callbacks

        

        #endregion

        #region private methods

        

        #endregion

        #region public methods

        public void ToggleStatusUpdate()
        {
            /// switch bewteen muted/unmuted status
            if (txtStatus.Text.Trim() == ButtonStatuses.AudioStatus.Muted.ToString())
            {
                txtStatus.Text = ButtonStatuses.AudioStatus.Unmuted.ToString();
            }
            else
            {
                txtStatus.Text = ButtonStatuses.AudioStatus.Muted.ToString();
            }
        }

        #endregion
    }
}
