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
    const link = document.createElement('a');
    link.href = "#";
    link.innerText = "Click here to open the folder with the downloaded file";
    link.onclick = () => {
      window.external.sendMessage(`OpenFolder:${filePath}`);
    };

    pTag.innerText = downloadMessage;
    pTag.style.color = 'green';
    statusDiv.innerHTML = '';
    statusDiv.appendChild(pTag);
    statusDiv.appendChild(link);
  } else if (message.startsWith("Error:")) {
    statusDiv.innerText = message.replace("Error:", "");
    statusDiv.style.color = 'red';
  }
});