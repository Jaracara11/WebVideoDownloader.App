function sendUrl() {
  const urlInput = document.getElementById('url-input');
  const videoUrl = urlInput.value.trim();

  if (!videoUrl) {
    document.getElementById('status').innerText = 'Please enter a valid URL.';
    return;
  }

  window.external.sendMessage(videoUrl);
  document.getElementById('status').innerText = 'Processing video download...';
}

window.external.receiveMessage((message) => {
  const statusElement = document.getElementById('status');
  statusElement.innerText = message;
});

