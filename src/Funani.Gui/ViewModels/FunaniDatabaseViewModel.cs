/*
 * Copyright (c) 2008-2016, Jaap de Haan <jaap.dehaan@color-of-code.de>
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 
 *   * Redistributions of source code must retain the above copyright notice,
 *     this list of conditions and the following disclaimer.
 *   * Redistributions in binary form must reproduce the above copyright
 *     notice, this list of conditions and the following disclaimer in the
 *     documentation and/or other materials provided with the distribution.
 *   * Neither the name of the "Color-Of-Code" nor the names of its
 *     contributors may be used to endorse or promote products derived from
 *     this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF
 * THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using Catel.MVVM;
using Funani.Api;

namespace Funani.Gui.ViewModels
{
    /// <summary>
    ///     FunaniDatabase view model.
    /// </summary>
    public class FunaniDatabaseViewModel : ViewModelBase
    {
        #region Fields

        private readonly IEngine _engine;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="FunaniDatabaseViewModel" /> class.
        /// </summary>
        public FunaniDatabaseViewModel(IEngine engine)
        {
            _engine = engine;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the title of the view model.
        /// </summary>
        /// <value>The title.</value>
        public override string Title
        {
            get { return "Funani Database View model"; }
        }

        public DatabaseInfo DatabaseInfo
        {
            get { return _engine.DatabaseInfo; }
        }

        public String DatabasePath
        {
            get { return _engine.DatabasePath; }
        }

        public long TotalFileCount
        {
            get { return _engine.TotalFileCount; }
        }

        #endregion

        #region Commands

        #endregion

        #region Methods

        #endregion
    }
}