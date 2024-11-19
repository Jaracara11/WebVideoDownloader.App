function sendUrl() {
  const urlInput = document.getElementById("url-input").value;

  if (urlInput) {
    window.external.sendMessage(urlInput);
  } else {
    document.getElementById("status").innerText = "Please provide a valid video URL.";
  }
}

window.external.receiveMessage((message) => {
  const statusDiv = document.getElementById('status');

  if (message.startsWith("FileReady:")) {
    const filePath = message.replace("FileReady:", "");
    const downloadLink = document.createElement('a');

    downloadLink.href = `file://${filePath}`;
    downloadLink.download = filePath.split(/[/\\]/).pop();
    downloadLink.innerText = 'Click here to download the video';
    downloadLink.style.display = 'block';

    statusDiv.innerHTML = '';
    statusDiv.appendChild(downloadLink);
  } else if (message.startsWith("Error:")) {
    statusDiv.innerText = message.replace("Error:", "");
  }
});