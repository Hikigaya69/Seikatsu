
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import Navbar from "@/components/Navbar";
import api from "../Utils/api";


export default function ForgotPassword() {

    const [email, setEmail] = useState("");
    const [loading, setLoading] = useState(false);
    const [sent, setSent] = useState(false);
    const [error, setError] = useState("");

    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (!email.includes("@")) {
            setError("Please enter a valid email address.");
            return;
        }

        setLoading(true);
        setError("");

        try {
            console.log("Calling forgot-password API");
            await api.post("/Auth/forgot-password", { email });
            setSent(true);
            console.log("email sent");
        } catch (err) {
            console.error(err);
            setError("Something went wrong. Please try again.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="min-h-screen">
            <Navbar />
            <div className="max-w-md mx-auto mt-20 bg-white shadow rounded-xl p-8">

                <h2 className="text-2xl font-bold text-center text-[#284b63] mb-4">
                    Forgot Password
                </h2>

                {sent ? (

                    <div className="text-center space-y-4">

                        <p className="text-green-600 font-medium">
                            If this email exists, a reset link has been sent 📩
                        </p>

                        <p className="text-sm text-gray-500">
                            Please check your inbox (and spam folder).
                        </p>

                        <button
                            onClick={() => navigate("/login")}
                            className="bg-[#284b63] text-white px-6 py-2 rounded-lg hover:opacity-90"
                        >
                            Back to Login
                        </button>

                    </div>

                ) : (

                    <form onSubmit={handleSubmit} className="space-y-4">

                        <input
                            type="email"
                            placeholder="Enter your email"
                            className="w-full border px-4 py-2 rounded-lg"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                        />

                        {error && (
                            <p className="text-sm text-red-500">{error}</p>
                        )}

                        <button
                            type="submit"
                            disabled={loading}
                            className="w-full bg-[#284b63] text-white py-2 rounded-lg hover:opacity-90 disabled:opacity-50"
                        >
                            {loading ? "Sending..." : "Send Reset Link"}
                        </button>

                    </form>

                )}

            </div>
        </div>


    );
}
