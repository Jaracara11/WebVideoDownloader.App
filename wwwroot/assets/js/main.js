function sendUrl() {
  const urlInput = document.getElementById("url-input").value;
  const downloadPath = prompt("Please enter the download path (e.g., C:\\Users\\YourName\\Downloads\\video.mp4):");

  if (urlInput && downloadPath) {
    const payload = {
      videoUrl: urlInput,
      downloadPath: downloadPath
    };

    window.external.sendMessage(JSON.stringify(payload));
  } else {
    document.getElementById("status").innerText = "Please provide both a URL and a valid download path.";
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