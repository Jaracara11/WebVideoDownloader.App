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
    const downloadMessage = `File successfully downloaded at: ${filePath}`;
    const pTag = document.createElement('p');

    pTag.innerText = downloadMessage;
    pTag.style.color = 'green';
    statusDiv.innerHTML = '';
    statusDiv.appendChild(pTag);
  } else if (message.startsWith("Error:")) {
    statusDiv.innerText = message.replace("Error:", "");
    statusDiv.style.color = 'red';
  }
});