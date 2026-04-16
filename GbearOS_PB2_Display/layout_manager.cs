// Copyright (c) 2026 Garrett Wyrick.



// [GbearOS Component]

// Name: layout_manager.cs

// Purpose: Tracks virtual-dashboard draw bounds for [COL:LEFT]/[COL:RIGHT]/[COL:FULL], auto [COL], and row-height accounting.

// PB Association: PB2

// Dependencies: VRageMath

// Key Methods: — (LayoutManager)



using System;

namespace IngameScript
{
    /// <summary>

    /// Tracks horizontal clip bounds and vertical scroll extent for [GbearOS] virtual panels.

    /// Side-by-side columns advance <see cref="PeakScrollY"/> by max(column heights), not the sum.

    /// </summary>

    public sealed class LayoutManager

    {

        public const string CmdCol = "COL";



        private float _panelW;

        private float _panelH;



        private float _committedEnd;

        private float _rowStart;

        private float _col1H;

        private float _col2H;

        /// <summary>0 = full width, 1 = left column, 2 = right column.</summary>

        private int _mode;



        public VRageMath.RectangleF CurrentBounds { get; private set; }



        public void Reset(float panelWidth, float panelHeight)

        {

            _panelW = panelWidth;

            _panelH = panelHeight;

            _committedEnd = 0f;

            _rowStart = 0f;

            _col1H = 0f;

            _col2H = 0f;

            _mode = 0;

            CurrentBounds = new VRageMath.RectangleF(0f, 0f, _panelW, _panelH);

        }



        /// <summary>Total scrollable height after the current command stream position.</summary>

        public float PeakScrollY

        {

            get

            {

                if (_mode == 0)

                    return _committedEnd;

                return Math.Max(_committedEnd, _rowStart + Math.Max(_col1H, _col2H));

            }

        }



        /// <summary>Y coordinate (virtual content space) where the next content block starts.</summary>

        public float NextContentY

        {

            get

            {

                if (_mode == 0)

                    return _committedEnd;

                if (_mode == 1)

                    return _rowStart + _col1H;

                return _rowStart + _col2H;

            }

        }



        /// <summary>Horizontal center for subheaders in the active column or full panel.</summary>

        public float ActiveCenterX

        {

            get { return CurrentBounds.X + CurrentBounds.Width * 0.5f; }

        }



        /// <summary>

        /// Applies <c>[COL]</c> (auto), <c>[COL:LEFT]</c>, <c>[COL:RIGHT]</c>, or <c>[COL:FULL]</c>.

        /// Parameterless <c>[COL]</c>: from full or right, flush row and start left; from left, switch to right.

        /// </summary>

        public void ApplyCol(string arg)

        {

            string a = arg == null ? "" : arg.Trim();

            if (a.Length == 0)

            {

                ApplyColAuto();

                return;

            }

            if (string.Equals(a, "FULL", StrIX.C))

            {

                EnterFullWidthSection();

                return;

            }

            if (string.Equals(a, "LEFT", StrIX.C))

            {

                ApplyColLeft();

                return;

            }

            if (string.Equals(a, "RIGHT", StrIX.C))

            {

                ApplyColRight();

                return;

            }

        }



        private void ApplyColAuto()

        {

            if (_mode == 0 || _mode == 2)

            {

                FlushRow();

                _rowStart = _committedEnd;

                _col1H = 0f;

                _col2H = 0f;

                _mode = 1;

                SetBoundsColumn(1);

                return;

            }

            if (_mode == 1)

            {

                _mode = 2;

                SetBoundsColumn(2);

            }

        }



        private void ApplyColLeft()

        {

            if (_mode == 1 || _mode == 2)

                FlushRow();

            _rowStart = _committedEnd;

            _col1H = 0f;

            _col2H = 0f;

            _mode = 1;

            SetBoundsColumn(1);

        }



        private void ApplyColRight()

        {

            if (_mode == 0)

            {

                _rowStart = _committedEnd;

                _col1H = 0f;

                _col2H = 0f;

            }

            else if (_mode == 1)

            {

            }

            else if (_mode == 2)

            {

                FlushRow();

                _rowStart = _committedEnd;

                _col1H = 0f;

                _col2H = 0f;

            }

            _mode = 2;

            SetBoundsColumn(2);

        }



        /// <summary>Modules that always span the full panel (REF, PWR, …) close an open two-column row first.</summary>

        public void EnterFullWidthSection()

        {

            FlushRow();

            _mode = 0;

            CurrentBounds = new VRageMath.RectangleF(0f, 0f, _panelW, _panelH);

        }



        public void RegisterContentHeight(float blockHeight)

        {

            if (_mode == 0)

            {

                _committedEnd += blockHeight;

                return;

            }

            if (_mode == 1)

                _col1H += blockHeight;

            else

                _col2H += blockHeight;

        }



        /// <summary>Call after processing all commands so a trailing two-column row contributes height once.</summary>

        public void FinalizeScrollExtent()

        {

            FlushRow();

        }



        private void FlushRow()

        {

            if (_mode == 0)

                return;

            float rowEnd = _rowStart + Math.Max(_col1H, _col2H);

            if (rowEnd > _committedEnd)

                _committedEnd = rowEnd;

            _mode = 0;

            _col1H = 0f;

            _col2H = 0f;

            CurrentBounds = new VRageMath.RectangleF(0f, 0f, _panelW, _panelH);

        }



        private void SetBoundsColumn(int col)

        {

            float half = _panelW * 0.5f;

            if (col == 1)

                CurrentBounds = new VRageMath.RectangleF(0f, 0f, half, _panelH);

            else

                CurrentBounds = new VRageMath.RectangleF(half, 0f, half, _panelH);

        }

    }
}
