'JASPER VAN BATEN / AMSTERCHEM PROVIDES THIS SOURCE CODE TO YOU ON AN "AS IS" BASIS 
'WITHOUT WARRANTIES OF ANY KIND. YOU EXPRESSLY AGREE THAT YOUR USE OF THE SOURCE CODE 
'IS AT YOUR SOLE RISK. TO THE FULL EXTENT PERMISSIBLE BY APPLICABLE LAW, 
'JASPER VAN BATEN / AMSTERCHEM DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, 
'BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
'PURPOSE. JASPER VAN BATEN / AMSTERCHEM WILL NOT BE LIABLE FOR ANY DAMAGES OF ANY KIND 
'ARISING FROM THE USE OF THE SOURCE CODE, INCLUDING, BUT NOT LIMITED TO DIRECT, 
'INDIRECT, INCIDENTAL, PUNITIVE, AND CONSEQUENTIAL DAMAGES.

Imports CAPEOPEN

Public Class XFlowFileNameParameter
    Inherits OptionParameter

    Sub New(unit As UnitOperationBase, mustExist As Boolean)
        MyBase.New(unit, "XFlowPath", "Path to the .xflow file stream description", CapeParamMode.CAPE_INPUT, Nothing, Nothing, False)
        _mustExist = mustExist
    End Sub

    Friend Overrides Function CheckValueValid(value As String, ByRef message As String) As Boolean
        Return ValidateString(value, message)
    End Function

    Public Overrides Function ValidateString(value As String, ByRef message As String) As Boolean
        Dim f As System.IO.FileInfo
        If value Is Nothing Then Return True
        Try
            f = New System.IO.FileInfo(value)
        Catch ex As Exception
            message = "Invalid file name: " + ex.Message
            Return False
        End Try
        If System.IO.Path.GetExtension(value) <> ".xflow" Then
            message = "Flow Exchange files must have the .xflow file extension"
            Return False
        End If
        Dim di As System.IO.DirectoryInfo
        Try
            di = New System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(value))
        Catch ex As Exception
            message = "Invalid directory: " + ex.Message
            Return False
        End Try
        If Not di.Exists Then
            message = "Directory """ + di.FullName + """ does not exist"
            Return False
        End If
        If _mustExist Then
            If Not f.Exists Then
                message = "Specified xflow file does not exist"
                Return False
            End If
        End If
        Return True
    End Function

    Dim _mustExist As Boolean

End Class