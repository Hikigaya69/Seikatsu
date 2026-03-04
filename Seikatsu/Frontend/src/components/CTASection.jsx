import { useNavigate } from "react-router-dom";
import "./CTASection.css";

function CTASection() {
  const navigate = useNavigate();

  return (
    <section className="cta-wrapper">
      <div className="cta-card">
        <h2>Ready to Start Shopping?</h2>

        <p>
          Join thousands of satisfied customers enjoying international
          groceries in Japan
        </p>

        <button
          className="cta-button"
          onClick={() => navigate("/register")}
        >
          Create Free Account
        </button>
      </div>
    </section>
  );
}

export default CTASection;