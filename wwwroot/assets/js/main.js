const downloadButton = document.getElementById("download-btn");
const statusDiv = document.getElementById("status");
const urlInput = document.getElementById("url-input");

const toggleButtonState = (isDisabled) => downloadButton.disabled = isDisabled;

const sendUrl = () => {
  const url = urlInput.value.trim();

  if (url) {
    toggleButtonState(true);
    statusDiv.innerText = "Downloading... Please wait.";
    window.external.sendMessage(url);
  } else {
    statusDiv.innerText = "Please provide a valid video URL.";
    toggleButtonState(false);
  }
};

const handleFileReady = (filePath) => {
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

  toggleButtonState(false);
};

const handleError = (errorMessage) => {
  statusDiv.innerText = errorMessage;
  statusDiv.style.color = 'red';
  toggleButtonState(false);
};

window.external.receiveMessage((message) => {
  if (message.startsWith("FileReady:")) {
    const filePath = message.replace("FileReady:", "");
    handleFileReady(filePath);
  } else if (message.startsWith("Error:")) {
    handleError(message.replace("Error:", ""));
  }
});

downloadButton.addEventListener('click', sendUrl);