'JASPER VAN BATEN / AMSTERCHEM PROVIDES THIS SOURCE CODE TO YOU ON AN "AS IS" BASIS 
'WITHOUT WARRANTIES OF ANY KIND. YOU EXPRESSLY AGREE THAT YOUR USE OF THE SOURCE CODE 
'IS AT YOUR SOLE RISK. TO THE FULL EXTENT PERMISSIBLE BY APPLICABLE LAW, 
'JASPER VAN BATEN / AMSTERCHEM DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, 
'BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
'PURPOSE. JASPER VAN BATEN / AMSTERCHEM WILL NOT BE LIABLE FOR ANY DAMAGES OF ANY KIND 
'ARISING FROM THE USE OF THE SOURCE CODE, INCLUDING, BUT NOT LIMITED TO DIRECT, 
'INDIRECT, INCIDENTAL, PUNITIVE, AND CONSEQUENTIAL DAMAGES.

Friend Class ProductFlashOptions

    Friend Enum FlashType
        Automatic
        TP
        PH
        PVF
    End Enum

    Friend Shared FlashTypeTag() As String = {"Auto", "TP", "PH", "PVF"} 'tags other than auto match AutoFlash option in XFlow file
    Friend Shared FlashTypeDescription() As String = {"Automatic", "Temperature and Pressure", "Pressure and Enthalpy", "Pressure and Vapor Fraction"}

    Shared tagDictionary As New Dictionary(Of String, FlashType)

    Shared Sub New()
        Dim i As Integer
        For i = 0 To FlashTypeTag.Length - 1
            tagDictionary(FlashTypeTag(i)) = i
        Next
    End Sub

    Friend Shared Function GetFlashType(tag As String) As FlashType
        Dim ft As FlashType = FlashType.Automatic
        tagDictionary.TryGetValue(tag, ft)
        Return ft
    End Function

End Class
