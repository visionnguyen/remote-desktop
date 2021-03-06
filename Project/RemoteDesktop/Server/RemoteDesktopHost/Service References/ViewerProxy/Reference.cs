﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RemoteDesktopServer.ViewerProxy {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ViewerProxy.IViewerService")]
    public interface IViewerService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IViewerService/PushScreenUpdate", ReplyAction="http://tempuri.org/IViewerService/PushScreenUpdateResponse")]
        void PushScreenUpdate(byte[] data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IViewerService/PushCursorUpdate", ReplyAction="http://tempuri.org/IViewerService/PushCursorUpdateResponse")]
        string PushCursorUpdate(byte[] data);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IViewerService/Ping", ReplyAction="http://tempuri.org/IViewerService/PingResponse")]
        string Ping();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IViewerServiceChannel : RemoteDesktopServer.ViewerProxy.IViewerService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ViewerServiceClient : System.ServiceModel.ClientBase<RemoteDesktopServer.ViewerProxy.IViewerService>, RemoteDesktopServer.ViewerProxy.IViewerService {
        
        public ViewerServiceClient() {
        }
        
        public ViewerServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ViewerServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ViewerServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ViewerServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public void PushScreenUpdate(byte[] data) {
            base.Channel.PushScreenUpdate(data);
        }
        
        public string PushCursorUpdate(byte[] data) {
            return base.Channel.PushCursorUpdate(data);
        }
        
        public string Ping() {
            return base.Channel.Ping();
        }
    }
}
