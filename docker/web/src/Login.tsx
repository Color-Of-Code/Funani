import React, { useState } from "react";

function Login() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  async function loginUserCallback(e: React.FormEvent) {
    e.preventDefault();
    console.log(`LOGIN USER CALLBACK: ${email} ${password}`);
  }

  return (
    <form onSubmit={(e) => loginUserCallback(e)}>
      <div>
        <input
          name="email"
          id="email"
          type="email"
          placeholder="Email"
          onChange={(e) => setEmail(e.target.value)}
          required
        />

        <input
          name="password"
          id="password"
          type="password"
          placeholder="Password"
          onChange={(e) => setPassword(e.target.value)}
          required
        />
        <button type="submit">Login</button>
      </div>
    </form>
  );
}

export default Login;
