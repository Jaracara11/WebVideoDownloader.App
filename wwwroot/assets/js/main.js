function callDotNet() {
  window.external.sendMessage('Hi .NET! 🤖');
}

window.external.receiveMessage(message => alert(message));