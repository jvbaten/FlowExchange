'JASPER VAN BATEN / AMSTERCHEM PROVIDES THIS SOURCE CODE TO YOU ON AN "AS IS" BASIS 
'WITHOUT WARRANTIES OF ANY KIND. YOU EXPRESSLY AGREE THAT YOUR USE OF THE SOURCE CODE 
'IS AT YOUR SOLE RISK. TO THE FULL EXTENT PERMISSIBLE BY APPLICABLE LAW, 
'JASPER VAN BATEN / AMSTERCHEM DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, 
'BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
'PURPOSE. JASPER VAN BATEN / AMSTERCHEM WILL NOT BE LIABLE FOR ANY DAMAGES OF ANY KIND 
'ARISING FROM THE USE OF THE SOURCE CODE, INCLUDING, BUT NOT LIMITED TO DIRECT, 
'INDIRECT, INCIDENTAL, PUNITIVE, AND CONSEQUENTIAL DAMAGES.

Public Class XFlowFileOptions
    Friend enthalpyReferencePhase As String 'nothing means auto
    Friend enthalpyReferenceT, enthalpyReferenceP As Double
    Friend writeCreatorComment, writeComments, writeAutoFlash, writeEnthalpy, writePhaseInfo As Boolean
    Friend Sub New()
        'defaults
        enthalpyReferencePhase = Nothing
        enthalpyReferenceT = 298.15
        enthalpyReferenceP = 101325
        writeCreatorComment = True
        writeComments = True
        writeAutoFlash = True
        writeEnthalpy = True
        writePhaseInfo = True
    End Sub
End Class
