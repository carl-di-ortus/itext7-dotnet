/*
This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: iText Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using iText.Test;

namespace iText.Commons.Utils {
    public class ProcessInfoTest : ExtendedITextTest {
        [NUnit.Framework.Test]
        public virtual void GetExitCodeTest() {
            int exitCode = 1;
            ProcessInfo processInfo = new ProcessInfo(exitCode, null, null);
            NUnit.Framework.Assert.AreEqual(exitCode, processInfo.GetExitCode());
        }

        [NUnit.Framework.Test]
        public virtual void GetProcessStdOutput() {
            String stdOutput = "output";
            ProcessInfo processInfo = new ProcessInfo(0, stdOutput, null);
            NUnit.Framework.Assert.AreEqual(stdOutput, processInfo.GetProcessStdOutput());
        }

        [NUnit.Framework.Test]
        public virtual void GetProcessErrOutput() {
            String stdOutput = "output";
            ProcessInfo processInfo = new ProcessInfo(0, null, stdOutput);
            NUnit.Framework.Assert.AreEqual(stdOutput, processInfo.GetProcessErrOutput());
        }
    }
}