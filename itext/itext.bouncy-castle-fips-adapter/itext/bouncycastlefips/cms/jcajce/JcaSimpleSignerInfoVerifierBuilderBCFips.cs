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
using Org.BouncyCastle.Cms.Jcajce;
using Org.BouncyCastle.Operator;
using Org.BouncyCastle.X509;
using iText.Bouncycastlefips.Cms;
using iText.Bouncycastlefips.Operator;
using iText.Commons.Bouncycastle.Cms;
using iText.Commons.Bouncycastle.Cms.Jcajce;
using iText.Commons.Utils;

namespace iText.Bouncycastlefips.Cms.Jcajce {
    /// <summary>
    /// Wrapper class for
    /// <see cref="Org.BouncyCastle.Cms.Jcajce.JcaSimpleSignerInfoVerifierBuilder"/>.
    /// </summary>
    public class JcaSimpleSignerInfoVerifierBuilderBCFips : IJcaSimpleSignerInfoVerifierBuilder {
        private readonly JcaSimpleSignerInfoVerifierBuilder verifierBuilder;

        /// <summary>
        /// Creates new wrapper instance for
        /// <see cref="Org.BouncyCastle.Cms.Jcajce.JcaSimpleSignerInfoVerifierBuilder"/>.
        /// </summary>
        /// <param name="verifierBuilder">
        /// 
        /// <see cref="Org.BouncyCastle.Cms.Jcajce.JcaSimpleSignerInfoVerifierBuilder"/>
        /// to be wrapped
        /// </param>
        public JcaSimpleSignerInfoVerifierBuilderBCFips(JcaSimpleSignerInfoVerifierBuilder verifierBuilder) {
            this.verifierBuilder = verifierBuilder;
        }

        /// <summary>Gets actual org.bouncycastle object being wrapped.</summary>
        /// <returns>
        /// wrapped
        /// <see cref="Org.BouncyCastle.Cms.Jcajce.JcaSimpleSignerInfoVerifierBuilder"/>.
        /// </returns>
        public virtual JcaSimpleSignerInfoVerifierBuilder GetVerifierBuilder() {
            return verifierBuilder;
        }

        /// <summary><inheritDoc/></summary>
        public virtual IJcaSimpleSignerInfoVerifierBuilder SetProvider(String provider) {
            verifierBuilder.SetProvider(provider);
            return this;
        }

        /// <summary><inheritDoc/></summary>
        public virtual ISignerInformationVerifier Build(X509Certificate certificate) {
            try {
                return new SignerInformationVerifierBCFips(verifierBuilder.Build(certificate));
            }
            catch (OperatorCreationException e) {
                throw new OperatorCreationExceptionBCFips(e);
            }
        }

        /// <summary>Indicates whether some other object is "equal to" this one.</summary>
        /// <remarks>Indicates whether some other object is "equal to" this one. Compares wrapped objects.</remarks>
        public override bool Equals(Object o) {
            if (this == o) {
                return true;
            }
            if (o == null || GetType() != o.GetType()) {
                return false;
            }
            iText.Bouncycastlefips.Cms.Jcajce.JcaSimpleSignerInfoVerifierBuilderBCFips that = (iText.Bouncycastlefips.Cms.Jcajce.JcaSimpleSignerInfoVerifierBuilderBCFips
                )o;
            return Object.Equals(verifierBuilder, that.verifierBuilder);
        }

        /// <summary>Returns a hash code value based on the wrapped object.</summary>
        public override int GetHashCode() {
            return JavaUtil.ArraysHashCode(verifierBuilder);
        }

        /// <summary>
        /// Delegates
        /// <c>toString</c>
        /// method call to the wrapped object.
        /// </summary>
        public override String ToString() {
            return verifierBuilder.ToString();
        }
    }
}