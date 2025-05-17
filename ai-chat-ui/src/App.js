import ChatUI from './ChatUI.jsx';

console.log("REACT_APP_API_URL at build:", process.env.REACT_APP_API_URL);

// Remove trailing slash to avoid double slashes or missing slashes
const apiUrl = (process.env.REACT_APP_API_URL || 'http://localhost:5000').replace(/\/+$/, '');
// const apiUrl = ("http://localhost:5000").replace(/\/+$/, "");
async function sendMessage(message, threadId) {
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
  if (!response.ok) throw new Error('Network response was not ok')
    else {
  console.log("TEST ********* Response status:", response.status);}
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
