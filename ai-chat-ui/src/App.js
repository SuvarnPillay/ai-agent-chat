import ChatUI from './ChatUI.jsx';

console.log(process.env.REACT_APP_API_URL)

// filepath: c:\dev\AI\Lab\Agent\proj_1\ai-agent-chat\ai-chat-ui\src\App.js
async function sendMessage(message, threadId) {
  const apiUrl = process.env.REACT_APP_API_URL || 'http://localhost:5000';
  const response = await fetch(
    `${apiUrl}/api/chat/chat`,
    {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ message, threadId }),
    }
  );
  if (!response.ok) throw new Error('Network response was not ok');
  const text = await response.text();
  return text;
}

function App() {
  return (
    <div className="App h-screen w-screen bg-gray-100">
      <ChatUI sendMessage={sendMessage} />
    </div>
  );
}

export default App;
