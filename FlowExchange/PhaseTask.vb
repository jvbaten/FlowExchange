﻿'JASPER VAN BATEN / AMSTERCHEM PROVIDES THIS SOURCE CODE TO YOU ON AN "AS IS" BASIS 
'WITHOUT WARRANTIES OF ANY KIND. YOU EXPRESSLY AGREE THAT YOUR USE OF THE SOURCE CODE 
'IS AT YOUR SOLE RISK. TO THE FULL EXTENT PERMISSIBLE BY APPLICABLE LAW, 
'JASPER VAN BATEN / AMSTERCHEM DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, 
'BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
'PURPOSE. JASPER VAN BATEN / AMSTERCHEM WILL NOT BE LIABLE FOR ANY DAMAGES OF ANY KIND 
'ARISING FROM THE USE OF THE SOURCE CODE, INCLUDING, BUT NOT LIMITED TO DIRECT, 
'INDIRECT, INCIDENTAL, PUNITIVE, AND CONSEQUENTIAL DAMAGES.

'phase summation task
Class PhaseTask
    'overall properties
    Friend overall As New List(Of PropertyInstance)
    'phase properties
    Friend phase As New List(Of PropertyInstance)
    'phase summation
    Friend molarSum As Double = Double.NaN
    Friend massSum As Double = Double.NaN
    'check if we calculated the property ok for the phase at hand
    Friend propOK As Boolean
End Class