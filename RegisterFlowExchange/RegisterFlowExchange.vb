'JASPER VAN BATEN / AMSTERCHEM PROVIDES THIS SOURCE CODE TO YOU ON AN "AS IS" BASIS 
'WITHOUT WARRANTIES OF ANY KIND. YOU EXPRESSLY AGREE THAT YOUR USE OF THE SOURCE CODE 
'IS AT YOUR SOLE RISK. TO THE FULL EXTENT PERMISSIBLE BY APPLICABLE LAW, 
'JASPER VAN BATEN / AMSTERCHEM DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, 
'BUT NOT LIMITED TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
'PURPOSE. JASPER VAN BATEN / AMSTERCHEM WILL NOT BE LIABLE FOR ANY DAMAGES OF ANY KIND 
'ARISING FROM THE USE OF THE SOURCE CODE, INCLUDING, BUT NOT LIMITED TO DIRECT, 
'INDIRECT, INCIDENTAL, PUNITIVE, AND CONSEQUENTIAL DAMAGES.

Imports FlowExchange
Imports System.Reflection
Imports System.IO

Module RegisterFlowExchange

    Sub RegisterXFileType(rootKey As Microsoft.Win32.RegistryKey)
        Dim key, sub1, sub2, sub3 As Microsoft.Win32.RegistryKey
        key = rootKey.CreateSubKey(".xflow")
        key.SetValue(Nothing, "Flow Exchange file")
        key.Close()
        key = rootKey.CreateSubKey("Flow Exchange file")
        sub1 = key.CreateSubKey("shell")
        sub2 = sub1.CreateSubKey("open")
        sub3 = sub2.CreateSubKey("command")
        sub3.SetValue(Nothing, """" + Path.Combine(Path.GetDirectoryName(GetType(RegisterFlowExchange).Assembly.Location), "XFlowViewer.exe") + """ ""%1""")
        sub3.Close()
        sub2.Close()
        sub1.Close()
        key.Close()
    End Sub

    Sub UnregisterXFileType(rootKey As Microsoft.Win32.RegistryKey)
        Try
            rootKey.DeleteSubKeyTree(".xflow")
        Catch ex As ArgumentException
        End Try
        Try
            rootKey.DeleteSubKeyTree("Flow Exchange file")
        Catch ex As ArgumentException
        End Try
    End Sub

    Sub Main()
        'parse the command line options
        Dim cmdLineArgs() As String = System.Environment.GetCommandLineArgs
        If cmdLineArgs.Length < 1 Then
usage:
            MsgBox("Usage:" + vbCrLf + "RegisterFlowExchange [options]" + vbCrLf + vbCrLf + "Options:" + vbCrLf + "AllUsers or A - registers for all users (default is current user)" + vbCrLf + "Unregister or U - removes registration (default is to register)" + vbCrLf + "Silent or S - hide message in case of succesful operation" + vbCrLf)
            Exit Sub
        End If
        Dim unregister As Boolean = False
        Dim allusers As Boolean = False
        Dim silent As Boolean = False
        Dim i As Integer
        For i = 1 To cmdLineArgs.Length - 1
            Dim s As String = cmdLineArgs(i)
            If (s.StartsWith("/")) Or (s.StartsWith("-")) Then s = s.Substring(1)
            If String.Compare(s, "AllUsers", True) = 0 Then
                allusers = True
            ElseIf String.Compare(s, "Unregister", True) = 0 Then
                unregister = True
            ElseIf String.Compare(s, "Silent", True) = 0 Then
                silent = True
            ElseIf String.Compare(s, "A", True) = 0 Then
                allusers = True
            ElseIf String.Compare(s, "U", True) = 0 Then
                unregister = True
            ElseIf String.Compare(s, "S", True) = 0 Then
                silent = True
            Else
                GoTo usage
            End If
        Next
        Try
            If unregister Then
                If allusers Then
                    XFlowSaver.UnregisterAllUsers(GetType(XFlowSaver))
                    XFlowLoader.UnregisterAllUsers(GetType(XFlowLoader))
                    UnregisterXFileType(Microsoft.Win32.Registry.LocalMachine.CreateSubKey("Software\Classes"))
                Else
                    XFlowSaver.UnregisterCurrentUser(GetType(XFlowSaver))
                    XFlowLoader.UnregisterCurrentUser(GetType(XFlowLoader))
                    UnregisterXFileType(Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\Classes"))
                End If
            Else
                If allusers Then
                    XFlowSaver.RegisterAllUsers(GetType(XFlowSaver))
                    XFlowLoader.RegisterAllUsers(GetType(XFlowLoader))
                    RegisterXFileType(Microsoft.Win32.Registry.LocalMachine.CreateSubKey("Software\Classes"))
                Else
                    XFlowSaver.RegisterCurrentUser(GetType(XFlowSaver))
                    XFlowLoader.RegisterCurrentUser(GetType(XFlowLoader))
                    RegisterXFileType(Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software\Classes"))
                End If
            End If
            If Not silent Then
                Dim sb As New System.Text.StringBuilder
                If unregister Then sb.Append("Unr") Else sb.Append("R")
                sb.Append("egistered the FlowExchange unit operations for ")
                If (allusers) Then sb.Append("all users") Else sb.Append("the current user")
                sb.Append(".")
                MsgBox(sb.ToString, MsgBoxStyle.Information, "FlowExchange:")
            End If
        Catch ex As Exception
            Dim sb As New System.Text.StringBuilder
            sb.Append("Failed to ")
            If unregister Then sb.Append("un")
            sb.Append("register the FlowExchange unit operations for ")
            If (allusers) Then sb.Append("all users") Else sb.Append("the current user")
            sb.Append(": ")
            sb.Append(ex.Message)
            MsgBox(sb.ToString, MsgBoxStyle.Critical, "FlowExchange:")
        End Try
    End Sub


End Module
