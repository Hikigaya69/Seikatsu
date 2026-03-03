import Navbar from "../components/Navbar";
import Hero from "../components/Hero";
import "./Landing.css";
import CountrySection from "../components/Countrysection";
import FeaturedProducts from "../components/FeaturedProducts";
import CTASection from "../components/CTASection";
import Footer from "../components/Footer";

function Landing() {
    return (
        <>
            <Navbar />
            <Hero />
            <CountrySection />
            <FeaturedProducts />
            <CTASection />
            <Footer />
        </>
    );
}

export default Landing;