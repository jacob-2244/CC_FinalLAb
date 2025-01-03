using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using Scanner.CompilerPhases;


namespace Scanner
{
    public partial class Scanner : Form
    {
        public Scanner()
        {
            InitializeComponent();
        }
        struct Token
        {
            public string token_type;
            public string token_value;
        }
        public enum States
        {
            START, INCOMMENT, INNUM, INID, INASSIGN, DONE
        };
        List<Token> tokensList = new List<Token>();

        private List<Token> get_tokens(string MyCode)
        {
            tokensList = new List<Token>();
            string state = States.START.ToString();
            string token_currentvalue = "";
            string token_currenttype = "";
            int counter = 0;
            MyCode = MyCode.Replace(" ", "  ");
            foreach (char c in MyCode + 1)
            {

                if (state == "START")
                {
                    if (c == '{')
                    {
                        state = "INCOMMENT";
                    }
                    else if (c == ':')
                    {
                        state = "INASSIGN";
                        token_currenttype = "ASSIGN";
                    }
                    else if (is_specialsymbol(c)) //to be modified 
                    {
                        state = "DONE";
                        token_currentvalue += c;
                        if (c == '+')
                            token_currenttype = "PLUS";
                        if (c == '-')
                            token_currenttype = "MINUS";
                        if (c == '*')
                            token_currenttype = "MULT";
                        if (c == '/')
                            token_currenttype = "DIV";
                        if (c == '=')
                            token_currenttype = "EQUAL";
                        if (c == '<')
                            token_currenttype = "LESSTHAN";
                        if (c == '(')
                            token_currenttype = "OPENBRACKET";
                        if (c == ')')
                            token_currenttype = "CLOSEDBRACKET";
                        if (c == ';')
                            token_currenttype = "SEMICOLON";

                    }
                    else if (is_identifier(c))
                    {
                        state = "INID";
                        token_currentvalue += c;
                        token_currenttype = "IDENTIFIER";
                    }
                    else if (is_number(c))
                    {
                        state = "INNUM";
                        token_currentvalue += c;
                        token_currenttype = "NUMBER";
                    }
                    else if (char.IsWhiteSpace(c))
                    {
                        state = "START";
                    }

                }

                else if (state == "INCOMMENT")
                {
                    if (c == '}')
                    {
                        state = "START";
                        token_currentvalue = "";
                    }
                    else state = "INCOMMENT";
                }

                else if (state == "INASSIGN")
                {
                    if (c == '=')
                    {
                        token_currentvalue = ":=";
                    }
                    state = "DONE";
                }

                else if (state == "INID")
                {
                    if (is_identifier(c))
                    {
                        token_currentvalue += c;
                        if (is_ReservedWord(token_currentvalue))
                        {
                            state = "DONE";
                            token_currenttype = token_currentvalue.ToUpper();
                        }
                    }
                    else if (is_number(c))
                    {
                        state = "INNUM";
                        token_currentvalue += c;
                    }
                    else
                    {
                        state = "DONE";
                    }



                }
                else if (state == "INNUM")
                {
                    if (is_number(c))
                    {
                        token_currentvalue += c;
                    }
                    else
                        state = "DONE";

                }
                else if (state == "DONE")
                {
                    Token t;
                    t.token_value = token_currentvalue;
                    t.token_type = token_currenttype;
                    tokensList.Add(t);
                    token_currentvalue = "";
                    state = "START";
                }
                counter++;
            }
            return tokensList;
        }
        private bool is_specialsymbol(char c)
        {
            if (c == '+' || c == '-' || c == '*' || c == '/' || c == '=' || c == '<' || c == '>' || c == '(' || c == ')' || c == ';') return true;
            return false;
        }
        private bool is_identifier(char c)
        {
            if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')) return true;
            return false;
        }
        private bool is_number(char c)
        {
            if (c >= '0' && c <= '9') return true;
            return false;
        }
        private bool is_ReservedWord(string s)
        {
            if (s == "if" || s == "else" || s == "then" || s == "else" || s == "end" || s == "repeat" || s == "until" || s == "read" || s == "write") return true;
            return false;
        }
        List<Token> total_tokens = new List<Token>();
        private void button2_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox1.Text)) { MessageBox.Show("Not avaliable code"); return; }
            label2.Text = "Tokens";
            textBox2.Visible = true;
            treeView1.Visible = false;
            //tokensList.AddRange(new Token[1000]);
            textBox2.Text = "";
            /*start implement*/
            string code = textBox1.Text;
            var textLines = code.Split('\n');
            foreach (var line in textLines)
            {
                List<Token> tokensList = new List<Token>();
                tokensList = get_tokens(line);
                total_tokens.AddRange(tokensList);
                foreach (Token t in tokensList)
                    textBox2.Text += t.token_value + "," + t.token_type + Environment.NewLine;
            }
            // textBox2.Text = textLines.Length.ToString();


            tokensList = total_tokens;
            button2.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        fileContent = reader.ReadToEnd();
                    }
                }
            }

            try
            {
                textBox1.Text = File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("invalid path");
            }




        }
        /***************************************************************************************************************/
        int p = 0;
        string[,] tokens = new string[5000, 2];
        private void button3_Click(object sender, EventArgs e)
        {
            // reset();
            label2.Text = "Syntax Tree";
            textBox2.Visible = false;
            treeView1.Visible = true;
            parse();
            button3.Enabled = false;
            //textBox1.Text = tokensList.Count.ToString();
        }
        private void reset()
        {
            p = 0;
            treeView1.Nodes.Clear();
            tokensList.Clear();
        }
        private void parse()
        {
            int i = 0;
            for (; i < tokensList.Count; i++)
            {
                tokens[i, 0] = tokensList[i].token_value;
                tokens[i, 1] = tokensList[i].token_type;
            }
            TreeNode node;
            node = treeView1.Nodes.Add("Start");
            stmt_seq(node);
            treeView1.ExpandAll();
            if (tokensList.Count == 0) { MessageBox.Show("no tokens "); reset(); }
        }
        private bool match(string exp, string input)
        {
            if (exp == input) { p++; return true; }
            else { MessageBox.Show("Missing" + exp); return false; }
        }

        private void statement(TreeNode node)
        {
            node = node.Nodes.Add("Statement");
            if (tokens[p, 0] == "if")
            {
                if_stmt(node);
            }
            else if (tokens[p, 0] == "repeat")
            {
                repeat_stmt(node);
            }
            else if (tokens[p, 0] == "read")
            {
                read_stmt(node);
            }
            else if (tokens[p, 0] == "write")
            {
                write_stmt(node);
            }
            else if (tokens[p, 1] == "IDENTIFIER")
            {
                assign_stmt(node);
            }
            else { node.Remove(); }
        }

        private void stmt_seq(TreeNode node)
        {
            statement(node);
            while (tokens[p, 0] == ";")
            {
                match(";", tokens[p, 0]);
                statement(node);
            }
        }

        private void if_stmt(TreeNode node)
        {
            node.Text = "IF";
            match("if", tokens[p, 0]);
            exp(node);
            match("then", tokens[p, 0]);
            Console.WriteLine(tokens[p, 0]);
            stmt_seq(node);
            if (tokens[p, 0] == "else")
            {
                match("else", tokens[p, 0]);
                stmt_seq(node);
            }
            match("end", tokens[p, 0]);
        }

        //repeat-stmt -> repeat stmt-seq until exp
        private void repeat_stmt(TreeNode node)
        {
            node.Text = "REPEAT";
            match("repeat", tokens[p, 0]);
            stmt_seq(node);
            match("until", tokens[p, 0]);

            exp(node);
        }

        //read-stmt -> read IDENTIFIER
        private void read_stmt(TreeNode node)
        {
            match("read", tokens[p, 0]);
            node.Text = "READ" + "(" + tokens[p, 0] + ")";
            match("IDENTIFIER", tokens[p, 1]);
        }

        //write-stmt -> write exp
        private void write_stmt(TreeNode node)
        {
            node.Text = "WRITE";
            match("write", tokens[p, 0]);
            exp(node);
        }

        //assign-stmt -> IDENTIFIER := exp
        private void assign_stmt(TreeNode node)
        {
            //TreeNode node;
            node.Text = "ASSIGN";
            node.Text += "(" + tokens[p, 0] + ")";
            match("IDENTIFIER", tokens[p, 1]);

            match(":=", tokens[p, 0]);
            exp(node);
        }

        //exp -> simple-exp [comparison-op simple exp]
        private void exp(TreeNode node)
        {
            node = node.Nodes.Add(tokens[p, 1]);
            simple_exp(node);
            if (tokens[p, 0] == "<" || tokens[p, 0] == "=")
            {
                node.Text = "op" + "(" + tokens[p, 0] + ")";
                comparison_op();
                simple_exp(node);
            }
        }

        //simple-exp -> term {addop term}
        private void simple_exp(TreeNode node)
        {
            term(node);
            while (tokens[p, 0] == "+" || tokens[p, 0] == "-")
            {
                node.Text = "op" + "(" + tokens[p, 0] + ")";
                addop();
                term(node);
            }
        }

        //factor -> number | IDENTIFIER | ( exp )
        private void factor(TreeNode node)
        {
            if (tokens[p, 1] == "NUMBER") { node.Nodes.Add("(" + tokens[p, 0] + ")"); match("NUMBER", tokens[p, 1]); }
            ////cout ( "Number" );

            else if (tokens[p, 0] == "(") // open
            {
                match("(", tokens[p, 0]); // open
                exp(node);

                if (tokens[p, 0] == ")") { match(")", tokens[p, 0]); } //closed
                else
                {

                    MessageBox.Show("Missed')'");
                }
            }
            else if (tokens[p, 1] == "IDENTIFIER")
            {
                node.Nodes.Add("(" + tokens[p, 0] + ")");
                match("IDENTIFIER", tokens[p, 1]);
            }
            else
            {
                MessageBox.Show("Missed number or IDENTIFIER or (");
            }
        }

        //term -> factor {mulop factor}
        private void term(TreeNode node)
        {
            factor(node);
            while (tokens[p, 0] == "*" || tokens[p, 0] == "/")
            {
                node.Text = "op" + "(" + tokens[p, 0] + ")";
                if (tokens[p, 0] == "*") match(tokens[p, 0], "*");
                if (tokens[p, 0] == "/") match(tokens[p, 0], "/");

                factor(node);
            }
        }

        //mulop -> * | /
        private void mulop()
        {
            if (tokens[p, 0] == "*") match(tokens[p, 0], "*");
            else if (tokens[p, 0] == "/") match(tokens[p, 0], "/");
            else
            {
                MessageBox.Show("Missed * or /");
            }
        }

        //addop -> + | -
        private void addop()
        {
            if (tokens[p, 0] == "+")
            {
                match(tokens[p, 0], "+");
            }
            else if (tokens[p, 0] == "-")
            {
                match("-", tokens[p, 0]);
            }
            else
            {
                MessageBox.Show("Missed + or -");
            }
        }

        //comparison-op -> < | =
        private void comparison_op()
        {
            if (tokens[p, 0] == "=")
            {
                match("=", tokens[p, 0]);
            }
            else if (tokens[p, 0] == "<")
            {
                match("<", tokens[p, 0]);
            }
            else
            {
                MessageBox.Show("missed comparison operator");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            reset();
            label2.Text = "Tokens";
            textBox1.Visible = true;
            treeView1.Visible = false;
            tokensList.Clear();
            total_tokens.Clear();
            Array.Clear(tokens, 0, tokens.Length);
            button2.Enabled = true;
            button3.Enabled = true;
            treeView1.Nodes.Clear();
            p = 0;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //Check if there are any tokens or parse tree nodes
            if (tokensList.Count == 0)
            {
                MessageBox.Show("No tokens available for semantic analysis.");
                return;
            }

            // Call the function to start semantic analysis
            performSemanticAnalysis();

        }



        private void performSemanticAnalysis()
        {
            // Semantic analysis logic goes here
            bool isValid = true;

            // Track declared identifiers
            HashSet<string> declaredVariables = new HashSet<string>();

            for (int i = 0; i < tokensList.Count; i++)
            {
                var token = tokensList[i];
                if (token.token_type == "IDENTIFIER")
                {
                    // If it's an identifier, check its context
                    if (isAssignment(token))
                    {
                        // If it's an assignment, check if the variable has been declared
                        string variableName = token.token_value;
                        if (!declaredVariables.Contains(variableName))
                        {
                            MessageBox.Show($"Error: Variable '{variableName}' is used before declaration.");
                            isValid = false;
                            break;
                        }
                    }
                    else
                    {
                        // Track declared variables
                        declaredVariables.Add(token.token_value);
                    }
                }
            }

            if (isValid)
            {
                MessageBox.Show("Semantic analysis completed successfully.");

            }
        }


        // Check if the current token represents an assignment operation
        private bool isAssignment(Token token)
        {
            return token.token_type == "ASSIGN";
        }


        //private bool match(string expected, string actual)
        //{
        //    if (expected == actual)
        //    {
        //        p++;
        //        return true;
        //    }
        //    else
        //    {
        //        MessageBox.Show($"Error: Expected '{expected}', but found '{actual}'");
        //        return false;
        //    }
        //}






    }
}

