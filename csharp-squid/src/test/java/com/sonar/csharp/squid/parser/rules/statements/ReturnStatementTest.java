/*
 * Copyright (C) 2010 SonarSource SA
 * All rights reserved
 * mailto:contact AT sonarsource DOT com
 */
package com.sonar.csharp.squid.parser.rules.statements;

import static com.sonar.sslr.test.parser.ParserMatchers.*;
import static org.junit.Assert.*;

import java.nio.charset.Charset;

import org.junit.Before;
import org.junit.Test;

import com.sonar.csharp.squid.CSharpConfiguration;
import com.sonar.csharp.squid.api.CSharpGrammar;
import com.sonar.csharp.squid.parser.CSharpParser;
import com.sonar.sslr.impl.Parser;

public class ReturnStatementTest {

  private final Parser<CSharpGrammar> p = CSharpParser.create(new CSharpConfiguration(Charset.forName("UTF-8")));
  private final CSharpGrammar g = p.getGrammar();

  @Before
  public void init() {
    p.setRootRule(g.returnStatement);

  }

  @Test
  public void testOk() {
    g.expression.mock();
    assertThat(p, parse("return;"));
    assertThat(p, parse("return expression;"));
  }

  @Test
  public void testRealLife() throws Exception {
    assertThat(p, parse("return RightContext is CollectionOperator ? base.LeftPrecedence + 10 : base.LeftPrecedence;"));
  }

}
